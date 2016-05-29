/*
    DOC:
    text - takes "20+(80*i)" instructors ("\n" beahviour unspecified yet)
*/
string TextPanelName = "LCD Centre 3 Base";

DisplayInterface display;
int val = 0;

Program() {
    var textPanel = GridTerminalSystem.GetBlockWithName(TextPanelName) as IMyTextPanel;
    display = new DisplayInterface(textPanel, this);
    //display.Show();
    Echo("INIT>>"+Runtime.CurrentInstructionCount+"/"+Runtime.MaxInstructionCount);
}

//  "\uE001\uE002\uE003\uE004\uE006\uE00E\uE00D\uE00F"
int runTick = 0;
static int fTime;
List<string> moj = new List<string>() {"R", "G", "Y"};
void Main(string argument) 
{
    runTick++;
    if (runTick == 1) {
        runTick = 0;
    }
    val += 1;
    
    fTime = DateTime.Now.Millisecond;
    
    display.Show();
    
    Echo("");
    Echo("Runtime:\n   "+(DateTime.Now.Millisecond-fTime) + " ms\n   "+Runtime.CurrentInstructionCount+"/"+
        Runtime.MaxInstructionCount+" instructions\n   "+Runtime.CurrentMethodCallCount+"/"+
        Runtime.MaxMethodCallCount+" methods call");
}

// ===================================================================================
class DisplayInterface {
    static Dictionary<char, List<List<int>>> convertTable = new Dictionary<char, List<List<int>>>();
    Dictionary<string, char> colorTable = new Dictionary<string, char>(); 
    List<char> grayList = new List<char>() {'\uE00F','\uE009','\uE00D','\uE00E','\uE006'};
    public Dictionary<int, DisplayElement> elementList = new Dictionary<int, DisplayElement>();
    char[] grayArray;
    int id_index = 0;
    IMyTextPanel panel;
    MyGridProgram meDebug;
    int x;
    int y;
    Single fontSize;
    List<List<char>> data = new List<List<char>>();

    public int brightness = 0;

    public DisplayInterface(IMyTextPanel pan, Single fSize = (Single)0.2, int sizeX = 80, int sizeY = 89) {
        panel = pan;
        fontSize = fSize;
        x = sizeX;
        y = sizeY;

        Initialize();
        ResetData();
    }

    public DisplayInterface(IMyTextPanel pan, MyGridProgram me, Single fSize = (Single)0.2, int sizeX = 80, 
                                    int sizeY = 89) {
        panel = pan;
        meDebug = me;
        fontSize = fSize; 
        x = sizeX; 
        y = sizeY;
        me.Echo("Debug ON");

        Initialize();
        ResetData();
    }

    public void Initialize() {
        convertTable.Add(' ', textToBlock("0 0 0\n0 0 0\n0 0 0\n0 0 0\n0 0 0"));
        convertTable.Add('A', textToBlock("0 1 0\n1 0 1\n1 1 1\n1 0 1\n1 0 1")); 
        convertTable.Add('B', textToBlock("1 1 0\n1 0 1\n1 1 0\n1 0 1\n1 1 0")); 
        convertTable.Add('C', textToBlock("0 1 0\n1 0 1\n1 0 0\n1 0 1\n0 1 0")); 
        convertTable.Add('D', textToBlock("1 1 0\n1 0 1\n1 0 1\n1 0 1\n1 1 0"));
        convertTable.Add('E', textToBlock("1 1 1\n1 0 0\n1 1 0\n1 0 0\n1 1 1"));
        convertTable.Add('F', textToBlock("1 1 1\n1 0 0\n1 1 0\n1 0 0\n1 0 0"));
        convertTable.Add('G', textToBlock("0 1 1\n1 0 0\n1 0 1\n1 0 1\n0 1 1"));
        convertTable.Add('H', textToBlock("1 0 1\n1 0 1\n1 1 1\n1 0 1\n1 0 1"));
        convertTable.Add('I', textToBlock("1 1 1\n0 1 0\n0 1 0\n0 1 0\n1 1 1"));
        convertTable.Add('J', textToBlock("0 0 1\n0 0 1\n0 0 1\n1 0 1\n1 1 1"));
        convertTable.Add('K', textToBlock("1 0 1\n1 0 1\n1 1 0\n1 0 1\n1 0 1"));
        convertTable.Add('L', textToBlock("1 0 0\n1 0 0\n1 0 0\n1 0 0\n1 1 1"));
        convertTable.Add('M', textToBlock("1 0 1\n1 1 1\n1 0 1\n1 0 1\n1 0 1"));
        convertTable.Add('N', textToBlock("1 1 0\n1 0 1\n1 0 1\n1 0 1\n1 0 1"));
        convertTable.Add('O', textToBlock("0 1 0\n1 0 1\n1 0 1\n1 0 1\n0 1 0"));
        convertTable.Add('P', textToBlock("1 1 0\n1 0 1\n1 1 0\n1 0 0\n1 0 0"));
        convertTable.Add('Q', textToBlock("0 1 0\n1 0 1\n1 0 1\n1 1 1\n0 1 1"));
        convertTable.Add('R', textToBlock("1 1 0\n1 0 1\n1 1 0\n1 0 1\n1 0 1"));
        convertTable.Add('S', textToBlock("0 1 1\n1 0 0\n0 1 0\n0 0 1\n1 1 0"));
        convertTable.Add('T', textToBlock("1 1 1\n0 1 0\n0 1 0\n0 1 0\n0 1 0"));
        convertTable.Add('U', textToBlock("1 0 1\n1 0 1\n1 0 1\n1 0 1\n1 1 1"));
        convertTable.Add('V', textToBlock("1 0 1\n1 0 1\n1 0 1\n0 1 0\n0 1 0"));
        convertTable.Add('W', textToBlock("1 0 1\n1 0 1\n1 0 1\n1 1 1\n1 0 1"));
        convertTable.Add('X', textToBlock("1 0 1\n1 0 1\n0 1 0\n1 0 1\n1 0 1"));
        convertTable.Add('Y', textToBlock("1 0 1\n1 0 1\n0 1 0\n0 1 0\n0 1 0"));
        convertTable.Add('Z', textToBlock("1 1 1\n0 0 1\n0 1 0\n1 0 0\n1 1 1"));

        convertTable.Add('0', textToBlock("1 1 1\n1 0 1\n1 0 1\n1 0 1\n1 1 1"));
        convertTable.Add('1', textToBlock("0 1 0\n1 1 0\n0 1 0\n0 1 0\n1 1 1"));
        convertTable.Add('2', textToBlock("1 1 1\n0 0 1\n1 1 1\n1 0 0\n1 1 1"));
        convertTable.Add('3', textToBlock("1 1 1\n0 0 1\n0 1 1\n0 0 1\n1 1 1"));
        convertTable.Add('4', textToBlock("1 0 1\n1 0 1\n1 1 1\n0 0 1\n0 0 1"));
        convertTable.Add('5', textToBlock("1 1 1\n1 0 0\n1 1 1\n0 0 1\n1 1 1"));
        convertTable.Add('6', textToBlock("1 1 1\n1 0 0\n1 1 1\n1 0 1\n1 1 1"));
        convertTable.Add('7', textToBlock("1 1 1\n0 0 1\n0 0 1\n0 0 1\n0 0 1"));
        convertTable.Add('8', textToBlock("1 1 1\n1 0 1\n1 1 1\n1 0 1\n1 1 1"));
        convertTable.Add('9', textToBlock("1 1 1\n1 0 1\n1 1 1\n0 0 1\n1 1 1"));

        convertTable.Add('.', textToBlock("0 0 0\n0 0 0\n0 0 0\n0 0 0\n0 1 0"));
        convertTable.Add(',', textToBlock("0 0 0\n0 0 0\n0 0 0\n0 1 0\n1 0 0"));
        convertTable.Add('!', textToBlock("0 1 0\n0 1 0\n0 1 0\n0 0 0\n0 1 0"));
        convertTable.Add('?', textToBlock("1 1 0\n0 1 0\n0 1 1\n0 0 0\n0 1 0"));
        convertTable.Add('_', textToBlock("0 0 0\n0 0 0\n0 0 0\n0 0 0\n1 1 1"));
        convertTable.Add(':', textToBlock("0 0 0\n0 1 0\n0 0 0\n0 1 0\n0 0 0"));
        convertTable.Add('"', textToBlock("1 0 1\n1 0 1\n0 0 0\n0 0 0\n0 0 0"));
        convertTable.Add('-', textToBlock("0 0 0\n0 0 0\n1 1 1\n0 0 0\n0 0 0"));
        convertTable.Add('+', textToBlock("0 0 0\n0 1 0\n1 1 1\n0 1 0\n0 0 0"));
        convertTable.Add('*', textToBlock("0 1 0\n1 1 1\n1 1 1\n0 1 0\n0 0 0"));
        convertTable.Add('%', textToBlock("1 0 1\n0 0 1\n0 1 0\n1 0 0\n1 0 1"));
        convertTable.Add('/', textToBlock("0 0 1\n0 1 0\n0 1 0\n1 0 0\n1 0 0"));
        convertTable.Add('\\', textToBlock("1 0 0\n0 1 0\n0 1 0\n0 0 1\n0 0 1"));
        convertTable.Add('>', textToBlock("1 0 0\n0 1 0\n0 0 1\n0 1 0\n1 0 0"));
        convertTable.Add('<', textToBlock("0 0 1\n0 1 0\n1 0 0\n0 1 0\n0 0 1"));
        convertTable.Add('\'', textToBlock("0 1 0\n0 1 0\n0 0 0\n0 0 0\n0 0 0"));
        convertTable.Add('(', textToBlock("0 0 1\n0 1 0\n0 1 0\n0 1 0\n0 0 1"));
        convertTable.Add(')', textToBlock("1 0 0\n0 1 0\n0 1 0\n0 1 0\n1 0 0"));
        convertTable.Add(';', textToBlock("0 0 0\n0 1 0\n0 0 0\n0 1 0\n1 0 0"));
        convertTable.Add('=', textToBlock("0 0 0\n1 1 1\n0 0 0\n1 1 1\n0 0 0"));
        convertTable.Add('[', textToBlock("0 1 1\n0 1 0\n0 1 0\n0 1 0\n0 1 1"));
        convertTable.Add(']', textToBlock("1 1 0\n0 1 0\n0 1 0\n0 1 0\n1 1 0"));
        convertTable.Add('{', textToBlock("0 1 1\n0 1 0\n1 0 0\n0 1 0\n0 1 1"));
        convertTable.Add('}', textToBlock("1 1 0\n0 1 0\n0 0 1\n0 1 0\n1 1 0"));
        convertTable.Add('^', textToBlock("0 1 0\n1 0 1\n0 0 0\n0 0 0\n0 0 0"));

        colorTable.Add("W", '\uE006');      // white
        colorTable.Add("G1", '\uE00E');      // light gray
        colorTable.Add("G2", '\uE00D');     // darker gray
        colorTable.Add("G3", '\uE009');     // even darker gray
        colorTable.Add("G4", '\uE00F');     // darkest gray
        colorTable.Add("G", '\uE001');        // green
        colorTable.Add("B", '\uE002');       // blue
        colorTable.Add("R", '\uE003');       // red
        colorTable.Add("Y", '\uE004');       // yellow
    }

    public void Process() {
        
    }

    public void RefreshData() {
        ResetData();

        var keys = new List<int>(elementList.Keys);
        var layerElements = new List<DisplayElement>();
        for (var i = 0; i < keys.Count; i++) {
            var layer = elementList[keys[i]].eLayer;
            var index = layerElements.Count;
            for (var el = 0; el < layerElements.Count; el++) {
                if (layer <= layerElements[el].eLayer) index = el+1;
                else break;
            }
            layerElements.Insert(index, elementList[i]);
        }
        for (var i = 0; i < layerElements.Count; i++) {
            MergeData(layerElements[i]);
        }
    }
    public void RefreshScreen(string strData = "") {
        if (strData == "") strData = ConvertData();
        if (panel != null)
            panel.WritePublicText(strData);
        else if (meDebug != null) meDebug.Echo("No valid text panel");
    }

    public void Show() {
        RefreshData();
        var toShow = ConvertData();
        if (panel.GetPublicText() != toShow) RefreshScreen(toShow);
        else if (meDebug != null) meDebug.Echo("Text already shown");
    }

    public void ResetData() {
        grayArray = new char[x];
        var gray = grayList[brightness]; 
        for (var i = 0; i < x; i++) {
            grayArray[i] = gray;
        }
        var newData = new List<List<char>>(); 
        for (var ty = 0; ty < y; ty++) {
            newData.Add(new List<char>(grayArray));
        }
        data = newData;
    }

    public string ConvertData(List<List<char>> dat = null) {
        if (dat == null) dat = data;
        string holder = "";
        for (var row = 0; row < dat.Count; row++) {
            holder += string.Join("", dat[row]);
            holder += "\n";
        }
        holder = holder.Substring(0, holder.Length-1);
        return holder;
    }
    public string ConvertData2(List<List<int>> dat = null) { 
        string holder = ""; 
        for (var row = 0; row < dat.Count; row++) { 
            for (var col = 0; col < dat[row].Count; col++) { 
                holder += dat[row][col]; 
            } 
            holder += "\n"; 
        } 
        holder = holder.Substring(0, holder.Length-1); 
        return holder; 
    }

    void MergeData(DisplayElement what) {
        var ccTable = what.colorConvertTable;
        var iposx = what.posX;
        var iposy = what.posY;
        var dat = what.eData;

        for (var iy = 0; iy < dat.Count; iy++) {
            for (var ix = 0; ix < dat[iy].Count; ix++) {
                if ((iy+iposy) < y && (iy+iposy) >= 0 && (ix+iposx) < x && (ix+iposx) >= 0) {
                    var d = dat[iy][ix];
                    if (d != 0)
                        data[iy+iposy][ix+iposx] = (colorTable.ContainsKey(ccTable[d]) ? 
                            colorTable[ccTable[d]] : colorTable["W"]);
                }
            }
        }
    }

    public List<List<int>> textToBlock(string inp) {  
        var blck = new List<List<int>>();  
        foreach (var line in inp.Split('\n')) {  
            var tmp = new List<int>();  
            var sp = line.Split(' ');  
            for (var cha = 0; cha < sp.Length; cha++) {  
                var charac = sp[cha];  
                var x = 0;  
                Int32.TryParse(charac, out x);  
                tmp.Add(x);  
            }  
            blck.Add(tmp);  
        }  
        return blck;  
    }
    public string Multiply(string what, int cnt) {
        string holder = "";
        for (var i = 0; i < cnt; i++) {
            holder += what;
        }
        return holder;
    }
    public int AddElementString(string data, int[] position, string color="Y", int layer=0, string bCol="") {
        var id = id_index;
        elementList.Add(id, 
                new DisplayElementString(meDebug, data, position, color, layer, bCol) as DisplayElement);
        id_index++;
        return id;
    }
    public int AddElementBar(float data, int[] pos, int[] size, string color="G", int layer=0, string fCol="G1",
                            float lvlColor2 = 2f, float lvlColor3 = 2f, string color2 = "Y", string color3 = "R") { 
        var id = id_index; 
        elementList.Add(id,  
                new DisplayElementBar(meDebug, data, pos, size, color, layer, fCol, lvlColor2,lvlColor3,color2,color3)
                    as DisplayElement); 
        id_index++; 
        return id; 
    }

    public class DisplayElement {
        public List<List<int>> eData;
        public int posX;
        public int posY;
        public int eLayer;
        public string eType;
        public Dictionary<int, string> colorConvertTable;
        internal MyGridProgram me;

        public DisplayElement(MyGridProgram m, string type,  int[] position, int layer = 0) {
            me = m;
            posX = position[0];
            posY = position[1];
            eLayer = layer;
            eType = type;
        }
        public virtual void Refresh() {
        }
    }

    public class DisplayElementString : DisplayElement {
        public string tColor;
        public string bColor;
        string eText;

        public DisplayElementString(MyGridProgram m, string dat, int[] position, string colorText = "Y", 
                        int layer = 0, string bgrdColor = "") 
                                : base(m, "TEXT", position, layer) {
            tColor = colorText;
            bColor = bgrdColor;
            colorConvertTable = new Dictionary<int, string>() {{1, tColor},{2, bColor}};
            eText = dat;

            eData = ConvertText(dat);
        }

        public List<List<int>> ConvertText(string str) { 
            int lineSpace = 2;                                 // how many rows between lines 
 
            int logic = 0; 
            List<List<int>> lst = new List<List<int>>(); 
            var strArr = str.Split('\n');
            for (var line = 0; line < strArr.Length; line++) { 
                for (var row = (5+lineSpace)*line; row < 5*(line+1)+lineSpace*line; row++) { 
                    lst.Add(new List<int>()); 
                    for (var letter = 0; letter < strArr[line].Length; letter++) { 
                        List<List<int>> letterData; 
                        if (new List<char>(convertTable.Keys).Contains(strArr[line][letter])) 
                            letterData = convertTable[strArr[line][letter]]; 
                        else letterData = convertTable[' ']; 
                        for (var i = 0; i < letterData[logic].Count; i++) { 
                            if (letterData[logic][i] == 1) lst[row].Add(1);
                            else if (bColor != "") lst[row].Add(2);
                            else lst[row].Add(0);
                        } 
                        lst[row].Add(bColor != "" ? 2 : 0);                       // space between letters 
                    } 
                    logic = ((logic+1)%5); 
                } 
                if (line < strArr.Length-1) for (var br = 0; br < lineSpace; br++) { 
                    lst.Add(new List<int>()); 
                } 
            } 
            return lst; 
        }
        public void Update(string textColor="", string backColor="", string text="") {
            if (textColor!="") tColor = textColor;
            if (backColor!="") bColor = backColor;
            if (text!="") {eData=ConvertText(text); eText=text;}
            colorConvertTable = new Dictionary<int, string>() {{1, tColor},{2, bColor}};
        }
        public override void Refresh() {
            eData=ConvertText(eText);
        }
        public string GetText() {
            return eText;
        }
    }
    public class DisplayElementBar : DisplayElement { 
        public string pColor;
        public string fColor;
        public string pColor2;
        public string pColor3;
        public float Color2;
        public float Color3;
        public int sizeX;
        public int sizeY;
        float progress;


        public DisplayElementBar(MyGridProgram m, float dat, int[] position, int[] size, string color = "G",  
                        int layer = 0, string frameColor = "Y", float lvlColor2 = 2f, float lvlColor3 = 2f, string color2 = "Y",
                        string color3 = "R") 
                                                : base(m, "BAR", position, layer) { 
            sizeX = size[0];
            sizeY = size[1];
            pColor = color;
            fColor = frameColor;
            pColor2 = color2;
            pColor3 = color3;
            Color2 = lvlColor2;
            Color3 = lvlColor3;
            progress = dat;
            colorConvertTable = new Dictionary<int, string>() {{1, pColor},{2, fColor},{3, pColor2},{4, pColor3}};

            eData = CreateProgressBar(dat);
        }

        public List<List<int>> CreateProgressBar(float data) { 
            List<List<int>> lst = new List<List<int>>();
            var borderLine = new int[sizeX];
            var barBody = new int[sizeX];
            for (var i = 0; i < sizeX; i++) {
                borderLine[i] = 2;

                // I'm sorry for this, when I do nested condition variable via "if () {}", I get error "illegal one-byte branch"
                // if i=first|last => 2; if i/max_i<data => (if i<color2_threshold => 1; if i< color3_threshold => 3; 4); 0
                barBody[i] = ((i != 0 && i != sizeX-1) ? (((float)i)/(sizeX-2)<=data ? (((float)i)/(sizeX-2)<Color2 ? 1 :
                    ((float)i)/(sizeX-2)<Color3 ? 3 : 4) : 0) : 2);
            }

            for (var iy = 0; iy < sizeY; iy++) {
                if (iy==0 || iy==sizeY-1) {
                    lst.Add(new List<int>(borderLine));
                    continue;
                } else lst.Add(new List<int>(barBody));
            }
            return lst; 
        }
        public void Update(float data=-1.077f, string color1="", string color2="", string color3="", float lvlColor2=-1.077f,
                        float lvlColor3=-1.077f, string frameColor="", int sizex=-1, int sizey=-1) {
            // -1.077f to lower chance that we actually want to update to this number

            if (data!=-1.077f) progress=data;
            if (lvlColor2!=-1.077f) Color2 = lvlColor2;
            if (lvlColor3!=-1.077f) Color3 = lvlColor3;
            if (color1!="") pColor = color1;
            if (color2!="") pColor2 = color2;
            if (color3!="") pColor3 = color3;
            if (frameColor!="") fColor = frameColor;
            if (sizex!=-1) sizeX = sizex;
            if (sizey!=-1) sizeY = sizey;
            eData = CreateProgressBar(progress);
            colorConvertTable = new Dictionary<int, string>() {{1, pColor},{2, fColor},{3, pColor2},{4, pColor3}};
        }
        public override void Refresh() { 
            eData=CreateProgressBar(progress); 
        }
        public float GetProgress() {
            return progress;
        }
    }
}