/*
Project Orleans Cloud Service SDK ver. 1.0
 
Copyright (c) Microsoft Corporation
 
All rights reserved.
 
MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the ""Software""), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Net;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orleans.Runtime.Host;

namespace OrleansXO.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        AzureSilo silo; 

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Do other silo initialization – for example: Azure diagnostics, etc
            
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            return base.OnStart();
        }

        public override void OnStop() { 
            silo.Stop();
            base.Stop();
        }
        
        public override void Run() { 
            var config = new ClusterConfiguration();
            config.StandardLoad();
            
            // It is IMPORTANT to start the silo not in OnStart but in Run. 
            // Azure may not have the firewalls open yet (on the remote silos) at the OnStart phase.
            silo = new AzureSilo();
            bool isSiloStarted = silo.Start(config);
            
            silo.Run(); // Call will block until silo is shutdown
        } 
    }
}