namespace Lesson11_02;

interface ISpellComponent
{
    SpellComponent SpellComponent();
}

public class SpellComponent
{
    
}

interface IBuffComponent
{
    BuffComponent BuffComponent();
}

public class BuffComponent
{
    
}

interface IItemComponent
{
    
}

interface IMoveComponent
{
    
}

public class Unit
{
    
}



public class Player: Unit, IBuffComponent, ISpellComponent
{
    private BuffComponent buffComponent;
    public BuffComponent BuffComponent()
    {
        return this.buffComponent;
    }
    
    private SpellComponent spellComponent;
    public SpellComponent SpellComponent()
    {
        return this.spellComponent;
    }
}

public class Monster: Unit
{
    private BuffComponent buffComponent;
    public BuffComponent BuffComponent()
    {
        return this.buffComponent;
    }
    
    private SpellComponent spellComponent;
    public SpellComponent SpellComponent()
    {
        return this.spellComponent;
    }
}

public class NPC: Unit
{
    private BuffComponent buffComponent;
    public BuffComponent BuffComponent()
    {
        return this.buffComponent;
    }
    
    private SpellComponent spellComponent;
    public SpellComponent SpellComponent()
    {
        return this.spellComponent;
    }
}

public class 法师: Player
{
    
}

public class 术士: Player
{
    
}

public class Boss: Monster
{
    
}

public class 普通怪: Monster
{
    
}

public class 巡逻NPC: NPC
{
    
}

public class 静止NPC: NPC
{
}

public class 对话NPC: NPC
{
}