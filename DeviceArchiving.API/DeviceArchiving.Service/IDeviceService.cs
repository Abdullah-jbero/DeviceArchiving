using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service
{
    public interface IDeviceService
    {
        void AddDevice(Device device);
        IEnumerable<Device> GetAllDevices();
        void UpdateDevice(Device device);
        void DeleteDevice(int id);
        

    }

}



