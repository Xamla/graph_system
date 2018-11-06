using System;
using System.Collections.Generic;
using System.Text;
using Xamla.Robotics.Types;

namespace Xamla.Laundrometer
{
    class TrajectoryHelpers
    {
        public static JointTrajectoryPoint InterpolateCubic(double t, JointTrajectoryPoint point0, JointTrajectoryPoint point1)
        {
            double t0 = point0.TimeFromStart.TotalSeconds;
            double t1 = point1.TimeFromStart.TotalSeconds;
            double dt = t1 - t0;

            JointSet jointSet = point1.Positions.JointSet;
            if (dt < 1e-6)
            {
                return new JointTrajectoryPoint(
                    timeFromStart: TimeSpan.FromSeconds(t0 + dt),
                    positions: point1.Positions,
                    velocities: JointValues.Zero(jointSet)
                );
            }

            double[] pos = point0.Positions.ToArray();
            double[] vel = point0.Velocities.ToArray();
            JointValues p0 = point0.Positions;
            JointValues p1 = point1.Positions;
            JointValues v0 = point0.Velocities;
            JointValues v1 = point1.Velocities;

            t = Math.Max(t - t0, 0);
            for (int i = 0; i < p0.Count; i++)
            {
                double a = p0[i];
                double b = v0[i];
                double c = (-3.0 * p0[i] + 3.0 * p1[i] - 2.0 * dt * v0[i] - dt * v1[i]) / Math.Pow(dt, 2);
                double d = (2.0 * p0[i] - 2.0 * p1[i] + dt * v0[i] + dt * v1[i]) / Math.Pow(dt, 3);
                pos[i] = a + b * t + c * Math.Pow(t, 2) + d * Math.Pow(t, 3);
                vel[i] = b + 2.0 * c * t + 3.0 * d * Math.Pow(t, 2);
            }

            return new JointTrajectoryPoint(
                timeFromStart: TimeSpan.FromSeconds(t),
                positions: new JointValues(jointSet, pos),
                velocities: new JointValues(jointSet, vel)
            );
        }

        public static JointTrajectoryPoint DeterminNextTarget(IJointTrajectory traj, TimeSpan simulatedTime, TimeSpan delay)
        {
            int index = 0;
            double time = Math.Max(simulatedTime.TotalSeconds - delay.TotalSeconds, 0);
            int pointCount = traj.Count;
            while (index < pointCount - 1 && time >= traj[Math.Min(index + 1, pointCount - 1)].TimeFromStart.TotalSeconds)
                index += 1;

            int k = Math.Min(index + 1, pointCount - 1);
            JointTrajectoryPoint p0 = traj[index];
            JointTrajectoryPoint p1 = traj[k];
            JointTrajectoryPoint q = InterpolateCubic(time, p0, p1);
            return q.WithTimeFromStart(simulatedTime);
        }

        public static IJointTrajectory MergeJointTrajectories(IJointTrajectory a, IJointTrajectory b, TimeSpan delayA, TimeSpan delayB)
        {
            JointSet unionJointSet = a.JointSet.Combine(b.JointSet);
            int numPointsA = a.Count;
            int numPointsB = b.Count;

            TimeSpan durationA = a[numPointsA - 1].TimeFromStart + delayA;
            TimeSpan durationB = b[numPointsB - 1].TimeFromStart + delayB;

            TimeSpan duration = durationA > durationB ? durationA : durationB;

            int maxNumberPoints = Math.Max(numPointsA, numPointsB);

            var result = new List<JointTrajectoryPoint>();
            for (int i = 0; i < maxNumberPoints; i++)
            {
                double t = i / (double)(maxNumberPoints - 1);
                TimeSpan simulatedTime = duration * t;
                JointTrajectoryPoint q_a = DeterminNextTarget(a, simulatedTime, delayA);
                JointTrajectoryPoint q_b = DeterminNextTarget(b, simulatedTime, delayB);
                JointTrajectoryPoint jtp = q_a.Merge(q_b);
                result.Add(jtp);
            }

            return new JointTrajectory(unionJointSet, result);
       }
    }
}
