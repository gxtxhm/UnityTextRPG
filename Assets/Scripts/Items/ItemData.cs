using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemData
{
    public string ItemType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Effect { get; set; }
    public int Duration { get; set; }
    public List<string> PositiveEffects { get; set; }  // ✅ 랜덤 포션의 긍정 효과 리스트
    public List<string> NegativeEffects { get; set; }  // ✅ 랜덤 포션의 부정 효과 리스트
}
