using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class Boss : Monster
{
    public bool IsDead=false;
    public Boss() : base()
    {
        
    }
    public override void Awake()
    {
        base.Awake();
        Name = "고대의 켈타르";
        OnDeadEvent += () => IsDead = true;
    }
    // 보스는 뭔가 더 추가하기  OnDeadEvent 여기에 , 엔딩 추가하기
}
