using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaTester
{
    public interface IFrameDrivable
    {
        /// <summary>
        /// 逻辑帧更新
        /// </summary>
        public void OnFrameUpdate();

    }
}
