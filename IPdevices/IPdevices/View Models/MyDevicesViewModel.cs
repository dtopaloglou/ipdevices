using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices.View_Models
{
    class MyDevicesViewModel : ViewModelBase
    {
        public ObservableCollection<UserDevice> Devices
        {
            get; set;
        }
        public MyDevicesViewModel()
        {
            Devices = new ObservableCollection<UserDevice>();
            Devices = getAllDevices();
        }

        private ObservableCollection<UserDevice> getAllDevices()
        {
            using (var db = new ipdevicesEntities())
            {
                int clientid = Properties.Settings.Default.clientid;
         
                var query = @"
                    SELECT c.*, p.IP, d.did, d.Name, p.Date
                        FROM client c, device d, ip p
                        WHERE c.uid = {0}
                        AND c.uid IN(
                            SELECT d1.uid
                            FROM device d1
                            WHERE c.uid = d1.uid
                                AND d.did = d1.did
                                AND d1.did IN(
                                    SELECT p1.did
                                    FROM ip p1
                                    WHERE p1.did = d1.did
                                        AND p.Date = (
                                            SELECT MAX(p2.date)
                                                FROM ip p2
                                                WHERE p2.did = d1.did

                                        )
                                )

                        )
                    ";
                var devices = db.Database.SqlQuery<UserDevice>(query, clientid).ToList();
                


               // var r = context.clients.Find(clientid).devices.Where(r => r.did == );

                return new ObservableCollection<UserDevice>(devices );
            }
        }
        /// <summary>
        /// This class was made in order to use it in the DataGrid shown above
        /// </summary>
        public class UserDevice
        {
            public UserDevice() { }

         
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public string IP { get; set; }
        }

    }
}
