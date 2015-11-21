using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Proland.Core.Producer
{
    /**
51 	 * A TileStorage that store tiles on CPU.
52 	 * @ingroup producer
53 	 * @authors Eric Bruneton, Antoine Begault
54 	 *
55 	 * @tparam T the type of each tile pixel component (e.g. char, float, etc).
56 	 */
    public class CPUTileStorage<T> : TileStorage
    {
        /**
68 	    * The data of the tile stored in this slot.
69 	    */
        public T data;
        /**
73 	    * The number of elements in the data array.
74 	    */
        public int size;
        /**
78 	    * Creates a new CPUSlot. This constructor creates a new array to store
79 	    * the tile data.
80 	    *
81 	    * @param owner the TileStorage that manages this slot.
82 	    * @param size the number of elements in the data array.
83 	    */

        public CPUSlot(TileStorage owner, int size)
        {
            this.data = new T.size; //new T[size]
            this.size = size;
        }

        ~CPUSlot()
        {
            if (data != null)
                {
                    delete[] data;  //Delete process?????
                }
        }

    }
}
