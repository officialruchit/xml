using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DeviceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Error: Invalid input. Program usage is as below.");
                Console.WriteLine("[DeviceUtil.exe] [XML file path]");
                Console.WriteLine("DeviceUtil.exe : Name of the executable file");
                Console.WriteLine("If anyone changes the name of the EXE, then the executable file name in usage should change accordingly.");
                Console.WriteLine("Terminate program.");
                return;
            }

            string xmlFilePath = args[0];

            // XML file validation
            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine("Error: File not exist. Please provide a valid file path.");
                Console.WriteLine("Terminate program.");
                return;
            }

            if (Path.GetExtension(xmlFilePath).ToLower() != ".xml")
            {
                Console.WriteLine("Error: Given file is not an XML file. The file extension is wrong.");
                Console.WriteLine("Terminate program.");
                return;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);

                // Empty XML file
                if (doc.DocumentElement.ChildNodes.Count == 0)
                {
                    Console.WriteLine("Error: The XML file is empty. Device data is not present in the file.");
                    Console.WriteLine("Terminate program.");
                    return;
                }
            }
            catch (XmlException)
            {
                Console.WriteLine("Error: Invalid XML file format.");
                Console.WriteLine("Terminate program.");
                return;
            }

            Dictionary<string, Dictionary<string, string>> devices = ParseXml(xmlFilePath);

            while (true)
            {
                Console.WriteLine("\nPlease select an option:");
                Console.WriteLine("[1] Show all devices");
                Console.WriteLine("[2] Search devices by serial number");
                Console.WriteLine("[3] Exit");

                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1":
                        ShowDevices(devices);
                        break;
                    case "2":
                        Console.Write("Enter serial number of the device: ");
                        string serialNumber = Console.ReadLine().Trim();
                        SearchDevice(devices, serialNumber);
                        break;
                    case "3":
                        Console.WriteLine("Program terminated.");
                        return;
                    default:
                        Console.WriteLine("Error: Invalid input. Please choose from the above options.");
                        break;
                }
            }
        }

        static Dictionary<string, Dictionary<string, string>> ParseXml(string filePath)
        {
            Dictionary<string, Dictionary<string, string>> devices = new Dictionary<string, Dictionary<string, string>>();

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name == "CommSetting")
                    {
                        foreach (XmlNode settingNode in childNode.ChildNodes)
                        {
                            deviceInfo[childNode.Name + "_" + settingNode.Name] = settingNode.InnerText;
                        }
                    }
                    else
                    {
                        deviceInfo[childNode.Name] = childNode.InnerText;
                    }
                }
                devices[node.Attributes["SrNo"].Value] = deviceInfo;
            }

            return devices;
        }

        static void ShowDevices(Dictionary<string, Dictionary<string, string>> devices)
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("{0,-5} {1,-20} {2,-20} {3,-20} {4,-20} {5,-10} {6,-10} {7,-10}", "No", "Serial Number", "IP Address", "Device Name", "Model Name", "Type", "Port", "SSL", "Password");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            int i = 1;
            foreach (var device in devices)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-20} {3,-20} {4,-20} {5,-10} {6,-10} {7,-10}", i++, device.Key, device.Value["Address"], device.Value["DevName"], device.Value["ModelName"], device.Value["Type"], device.Value["CommSetting_PortNo"], device.Value["CommSetting_UseSSL"], device.Value["CommSetting_Password"]);
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
        }

        static void SearchDevice(Dictionary<string, Dictionary<string, string>> devices, string serialNumber)
        {
            if (devices.ContainsKey(serialNumber))
            {
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-10} {5,-10} {6,-10}", "Serial Number", "IP Address", "Device Name", "Model Name", "Type", "Port", "SSL", "Password");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-10} {5,-10} {6,-10}", serialNumber, devices[serialNumber]["Address"], devices[serialNumber]["DevName"], devices[serialNumber]["ModelName"], devices[serialNumber]["Type"], devices[serialNumber]["CommSetting_PortNo"], devices[serialNumber]["CommSetting_UseSSL"], devices[serialNumber]["CommSetting_Password"]);
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Device not found.");
            }
        }
    }
}
