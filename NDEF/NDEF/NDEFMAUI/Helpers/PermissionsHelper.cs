using System;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace NDEFMAUI.Helpers
{
    public static class PermissionsHelper
    {
        public static async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>()
            where T : BasePermission, new()
        {
            var status = await CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await RequestAsync<T>();
            }
            return status;
        }
    }
}

