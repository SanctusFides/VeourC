using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veour.ViewModel;

namespace Veour.Services
{
    public static class Utility
    {

        public static ObservableCollection<string> LoadCityList()
        {
            ObservableCollection<string> Cities  = new ObservableCollection<string>();
            try
            {

                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string File = Path.Combine(CurrentDirectory, @"Assets\Files\locations-ranked.txt");
                string FilePath = Path.GetFullPath(File);
                using StreamReader sr = new StreamReader(FilePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Cities.Add(line);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not read the file");
                Debug.WriteLine(e.Message);
            }
            return Cities;
        }
    }
}
