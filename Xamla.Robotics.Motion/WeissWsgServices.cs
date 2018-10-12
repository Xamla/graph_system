﻿using Rosvita.RosMonitor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;
using Uml.Robotics.Ros;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Xamla.Robotics.Motion
{
    using wsg_50 = Messages.wsg_50;

    public class WeissWsgServices
        : IWeissWsgServices
        , IDisposable
    {
        private IRosClientLibrary rosClient;
        private WeissWsgPropertiesModel properties;
        private IMotionService motionService;
        private ServiceClient<wsg_50.GetGripperStatus> wsgStatusClient;
        private ServiceClient<wsg_50.SetValue> wsgSetAccelerationClient;
        private readonly object gate = new object();

        public WeissWsgServices(IRosClientLibrary rosClient, WeissWsgPropertiesModel properties)
        {
            this.rosClient = rosClient;
            this.properties = properties;

            rosClient.OnRosMasterConnected += RosClientOnRosMasterConnected;
            rosClient.OnRosMasterDisconnected += RosClientOnRosMasterDisconnected;
        }

        public async Task AcknowledgeError()
        {
            CancellationToken cancel = default(CancellationToken);
            var result = await this.motionService.WsgGripperCommand(properties.ControlAction, WsgCommand.AcknowledgeError, 0, 0, 0, true, cancel);
        }

        public void Dispose()
        {
            this.motionService?.Dispose();
            this.motionService = null;
            this.wsgStatusClient?.Dispose();
            this.wsgStatusClient = null;
            this.wsgSetAccelerationClient?.Dispose();
            this.wsgSetAccelerationClient = null;

            rosClient.OnRosMasterConnected -= RosClientOnRosMasterConnected;
            rosClient.OnRosMasterDisconnected -= RosClientOnRosMasterDisconnected;
        }

        public async Task<WeissWsgPropertiesModel> GetProperties()
        {
            return this.properties;
        }

        public async Task<wsg_50.Status> GetStatus()
        {
            if (this.wsgStatusClient != null)
            {
                var srv = new wsg_50.GetGripperStatus();
                if (!await this.wsgStatusClient.CallAsync(srv))
                    throw new ServiceCallFailedException(this.properties.StatusService);

                return srv.resp.status;
            }
            else
            {
                throw new ServiceCallFailedException(this.properties.StatusService);
            }
        }

        public async Task Grasp(double position, double speed, double force)
        {
            CancellationToken cancel = default(CancellationToken);
            var result = await this.motionService.WsgGripperCommand(properties.ControlAction, WsgCommand.Grasp, position, speed, force, true, cancel);
        }

        public async Task Homing()
        {
            CancellationToken cancel = default(CancellationToken);
            var result = await this.motionService.WsgGripperCommand(properties.ControlAction, WsgCommand.Homing, 0, 0, 0, true, cancel);
        }

        public async Task Move(double position, double speed, double force, bool stopOnBlock = true)
        {
            CancellationToken cancel = default(CancellationToken);
            var result = await this.motionService.WsgGripperCommand(properties.ControlAction, WsgCommand.Move, position, speed, force, stopOnBlock, cancel);
        }

        public async Task Release(double position, double speed)
        {
            CancellationToken cancel = default(CancellationToken);
            var result = await this.motionService.WsgGripperCommand(properties.ControlAction, WsgCommand.Release, position, speed, 0, true, cancel);
        }

        public async Task<int> SetAcceleration(double acceleration)
        {
            var srv = new wsg_50.SetValue();
            srv.req.val = acceleration;
            if (!await this.wsgSetAccelerationClient.CallAsync(srv))
                throw new ServiceCallFailedException(this.properties.SetAccelerationService);

            return srv.resp.error;
        }

        public async Task Stop()
        {
            CancellationToken cancel = default(CancellationToken);
            var result = await this.motionService.WsgGripperCommand(properties.ControlAction, WsgCommand.Stop, 0, 0, 0, true, cancel);
        }

        private void RosClientOnRosMasterConnected(object sender, EventArgs e)
        {
            lock (gate)
            {
                var nodeHandle = rosClient.GlobalNodeHandle;
                this.motionService = new MotionService(nodeHandle);
                this.wsgStatusClient = nodeHandle.ServiceClient<wsg_50.GetGripperStatus>(properties.StatusService, true);
                this.wsgSetAccelerationClient = nodeHandle.ServiceClient<wsg_50.SetValue>(properties.SetAccelerationService, true);
            }
        }

        private void RosClientOnRosMasterDisconnected(object sender, EventArgs e)
        {
            lock (gate)
            {
                this.motionService?.Dispose();
                this.motionService = null;
                this.wsgStatusClient?.Dispose();
                this.wsgStatusClient = null;
                this.wsgSetAccelerationClient?.Dispose();
                this.wsgSetAccelerationClient = null;
            }
        }
    }
}
