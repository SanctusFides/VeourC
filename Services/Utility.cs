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
            ObservableCollection<string> cities  = [];
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string file = Path.Combine(currentDirectory, @"Assets\Files\locations-ranked.txt");
                string filePath = Path.GetFullPath(file);
                using StreamReader sr = new StreamReader(filePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    cities.Add(line);
                }
            }
            catch (Exception e)
            {
                // TODO log error
                Debug.WriteLine("Could not read the file");
                Debug.WriteLine(e.Message);
            }
            return cities;
        }
    }
}
