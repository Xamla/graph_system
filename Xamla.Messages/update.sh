# temporarily move ros_definitions to the parent directory since YAMLParser removes all sub-directories during message generation.
mv ros_definitions ..
pushd ../ThirdParty/ROS.NET/YAMLParser
dotnet run -f netcoreapp2.1 -- -m ../../../ros_definitions -c Debug -a ../Uml.Robotics.Ros.Messages/bin/Debug/netcoreapp2.1/Uml.Robotics.Ros.Messages.dll -n Messages -o ../../../Xamla.Messages
popd
# remove unused files
rm .gitignore
rm Messages.csproj
rm MessageTypes.cs
# restore ros_definitions folder
mv ../ros_definitions .
