public class Pick {
    private int digPower;
    private int level;
    private int maxLevel;
    private int price;
    
    public Pick(int digPower, int level, int maxLevel, int price) {
        this.digPower = digPower;
        this.level = level;
        this.maxLevel = maxLevel;
        this.price = price;
    }
    
    public int DigPower {
        get => digPower;
        set => digPower = value;
    }
    
    public int Level {
        get => level;
        set => level = value;
    }
    
    public int MaxLevel {
        get => maxLevel;
        set => maxLevel = value;
    }
    
    public int Price {
        get => price;
        set => price = value;
    }
    
    public bool UpgradeIfPossible() {
        if (level < maxLevel) {
            level++;
            digPower++;
            price *= 2;
            return true;
        }

        return false;
    }

    public void Reset() {
        level = 1;
        digPower = 1;
        price = 10;
    }
    
    public override string ToString() {
        return $"DigPower: {digPower}, Level: {level}, MaxLevel: {maxLevel}, Price: {price}";
    }
}