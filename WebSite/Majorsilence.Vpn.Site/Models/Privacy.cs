using System;

namespace Majorsilence.Vpn.Site.Models
{
    public class Privacy
    {
        public Privacy()
        {
            var t = new LibLogic.Site.Privacy();
            _priv = t.CurrentPrivacy();
        }

        private readonly LibPoco.Privacy _priv;
        public LibPoco.Privacy CurrentPrivacy
        {
            get
            {
                return _priv;
            }
        }

    }
}

