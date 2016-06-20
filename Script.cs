/*
    DOCUMENTATION:
    20+(80)*i - TEXT
*/
string TextPanelName = "LCD Centre 3 Base";

DisplayInterface display;
int uid;
int uid2;
Program() {
    var textPanel = GridTerminalSystem.GetBlockWithName(TextPanelName) as IMyTextPanel;
    display = new DisplayInterface(textPanel, this, Runtime, 0.2f);
    uid2 = display.AddElementString("", new int[] {9, 35}, "R", -6);
    display.AddElementString("ENERGY:\n 2.35 MW", new int[] {1, 1}, "Y", 0);
    display.AddElementString("(+230.5 kW)", new int[] {34, 8}, "G", 0);
    display.AddElementCustom(new int[] {1,20}, 
"00111000011100\n01222100122210\n12222211222221\n12222222222221\n12222222222221\n"+
"01222222222210\n00122222222100\n"+
"00012222221000\n00001222210000\n00000122100000\n00000011000000",
        new Dictionary<int, string>() {{1, "Y"}, {2, "R"}});
    Echo("INIT>>"+Runtime.CurrentInstructionCount+"/"+Runtime.MaxInstructionCount);
}

int runTick = 0;
static int fTime;
void Main(string argument) 
{
    runTick++;
    if (runTick == 1) {
        runTick = 0;
    }
    fTime = DateTime.Now.Millisecond;

    display.Process();

    Echo("Runtime:\n   "+(DateTime.Now.Millisecond-fTime) + " ms\n   "+Runtime.CurrentInstructionCount+"/"+
        Runtime.MaxInstructionCount+" instructions\n   "+Runtime.CurrentMethodCallCount+"/"+
        Runtime.MaxMethodCallCount+" methods call");
}

// ===================================================================================
class DisplayInterface {   
    public string bgrColor = "G4";   
    int updateTick = 1;  
    List<string> grayList = new List<string>() {"'   '","!|!|!","\uE00F","\uE009","\uE00D","\uE00E","\uE006"}; 
    // ======================- NO MORE EDITING IN THIS CLASS -========================= 
    // ==============- (you can however add new characters to convertTable -================== 
 
    static Dictionary<char, List<List<int>>> convertTable = new Dictionary<char, List<List<int>>>();   
    Dictionary<string, string> colorTable = new Dictionary<string, string>();     
    public Dictionary<int, DisplayElement> elementList = new Dictionary<int, DisplayElement>();   
    string[] bgrArray;   
    int id_index = 0;   
    IMyTextPanel panel;   
    MyGridProgram meDebug;   
    IMyGridProgramRuntimeInfo runtime;   
    int x;   
    int y;   
    Single fontSize;   
    List<List<string>> data = new List<List<string>>();  
    int tick = 0;  
   
    // Anti script-complexity system: should prevent "Script is too complex" error to be raised, instead it will   
    //   split the execution of script in several steps (speed of update will be decreased)   
    int ASCS_ind = 0;       // if is higher than 0, it means that last run was complex and we need to finish it.   
    List<DisplayElement> ASCS_lst = null;   // save sorted list to save Instructions   
   
    public DisplayInterface(IMyTextPanel pan, IMyGridProgramRuntimeInfo rt, Single fSize = (Single)0.2, int sizeX = 80, int sizeY = 89) {   
        panel = pan;   
        fontSize = fSize;   
        x = sizeX;   
        y = sizeY;   
        runtime = rt;   
   
        Initialize();   
        ResetData();   
    }   
   
    public DisplayInterface(IMyTextPanel pan, MyGridProgram me, IMyGridProgramRuntimeInfo rt, Single fSize = (Single)0.2, int sizeX = 80,    
                                    int sizeY = 89) {   
        panel = pan;   
        meDebug = me;   
        fontSize = fSize;    
        x = sizeX;    
        y = sizeY;   
        runtime = rt;   
        me.Echo("Debug ON");   
   
        Initialize();   
        ResetData();   
    }   
   
    public void Initialize() {   
        convertTable.Add(' ', textToBlock("000/000/000/000/000"));    
        convertTable.Add('A', textToBlock("010/101/111/101/101"));     
        convertTable.Add('B', textToBlock("110/101/110/101/110"));     
        convertTable.Add('C', textToBlock("010/101/100/101/010"));     
        convertTable.Add('D', textToBlock("110/101/101/101/110"));    
        convertTable.Add('E', textToBlock("111/100/110/100/111"));    
        convertTable.Add('F', textToBlock("111/100/110/100/100"));    
        convertTable.Add('G', textToBlock("011/100/101/101/011"));    
        convertTable.Add('H', textToBlock("101/101/111/101/101"));    
        convertTable.Add('I', textToBlock("111/010/010/010/111"));    
        convertTable.Add('J', textToBlock("001/001/001/101/111"));    
        convertTable.Add('K', textToBlock("101/101/110/101/101"));    
        convertTable.Add('L', textToBlock("100/100/100/100/111"));    
        convertTable.Add('M', textToBlock("101/111/101/101/101"));    
        convertTable.Add('N', textToBlock("111/101/101/101/101"));    
        convertTable.Add('O', textToBlock("010/101/101/101/010"));    
        convertTable.Add('P', textToBlock("110/101/110/100/100"));    
        convertTable.Add('Q', textToBlock("010/101/101/111/011"));    
        convertTable.Add('R', textToBlock("110/101/110/101/101"));    
        convertTable.Add('S', textToBlock("011/100/010/001/110"));    
        convertTable.Add('T', textToBlock("111/010/010/010/010"));    
        convertTable.Add('U', textToBlock("101/101/101/101/111"));    
        convertTable.Add('V', textToBlock("101/101/101/010/010"));    
        convertTable.Add('W', textToBlock("101/101/101/111/101"));    
        convertTable.Add('X', textToBlock("101/101/010/101/101"));    
        convertTable.Add('Y', textToBlock("101/101/010/010/010"));    
        convertTable.Add('Z', textToBlock("111/001/010/100/111"));    
   
        convertTable.Add('a', textToBlock("000/111/001/111/111"));     
        convertTable.Add('b', textToBlock("100/111/101/101/111"));     
        convertTable.Add('c', textToBlock("000/111/100/100/111"));     
        convertTable.Add('d', textToBlock("001/111/101/101/111"));     
        convertTable.Add('e', textToBlock("000/111/111/100/111"));     
        convertTable.Add('f', textToBlock("011/010/111/010/010"));     
        convertTable.Add('g', textToBlock("000/111/111/001/111"));     
        convertTable.Add('h', textToBlock("100/111/101/101/101"));     
        convertTable.Add('i', textToBlock("010/000/010/010/111"));     
        convertTable.Add('j', textToBlock("010/000/010/010/110"));     
        convertTable.Add('k', textToBlock("100/100/101/110/101"));     
        convertTable.Add('l', textToBlock("010/010/010/010/001"));     
        convertTable.Add('m', textToBlock("000/101/111/101/101"));     
        convertTable.Add('n', textToBlock("000/111/101/101/101"));     
        convertTable.Add('o', textToBlock("000/111/101/101/111"));     
        convertTable.Add('p', textToBlock("000/111/101/111/100"));     
        convertTable.Add('q', textToBlock("000/111/101/111/001"));     
        convertTable.Add('r', textToBlock("000/111/100/100/100"));     
        convertTable.Add('s', textToBlock("000/011/110/011/110"));     
        convertTable.Add('t', textToBlock("010/111/010/010/011"));     
        convertTable.Add('u', textToBlock("000/101/101/101/111"));     
        convertTable.Add('v', textToBlock("000/101/101/101/010"));     
        convertTable.Add('w', textToBlock("000/101/101/111/111"));     
        convertTable.Add('x', textToBlock("000/101/101/010/101"));     
        convertTable.Add('y', textToBlock("000/101/111/001/111"));     
        convertTable.Add('z', textToBlock("000/111/011/110/111"));    
    
        convertTable.Add('0', textToBlock("111/101/101/101/111"));    
        convertTable.Add('1', textToBlock("010/110/010/010/111"));    
        convertTable.Add('2', textToBlock("111/001/111/100/111"));    
        convertTable.Add('3', textToBlock("111/001/011/001/111"));    
        convertTable.Add('4', textToBlock("101/101/111/001/001"));    
        convertTable.Add('5', textToBlock("111/100/111/001/111"));    
        convertTable.Add('6', textToBlock("111/100/111/101/111"));    
        convertTable.Add('7', textToBlock("111/001/001/001/001"));    
        convertTable.Add('8', textToBlock("111/101/111/101/111"));    
        convertTable.Add('9', textToBlock("111/101/111/001/111"));    
    
        convertTable.Add('.', textToBlock("000/000/000/000/010"));    
        convertTable.Add(',', textToBlock("000/000/000/010/100"));    
        convertTable.Add('!', textToBlock("010/010/010/000/010"));    
        convertTable.Add('?', textToBlock("110/010/011/000/010"));    
        convertTable.Add('_', textToBlock("000/000/000/000/111"));    
        convertTable.Add(':', textToBlock("000/010/000/010/000"));    
        convertTable.Add('"', textToBlock("101/101/000/000/000"));    
        convertTable.Add('-', textToBlock("000/000/111/000/000"));    
        convertTable.Add('+', textToBlock("000/010/111/010/000"));    
        convertTable.Add('*', textToBlock("010/111/111/010/000"));    
        convertTable.Add('%', textToBlock("101/001/010/100/101"));    
        convertTable.Add('/', textToBlock("001/010/010/100/100"));    
        convertTable.Add('\\', textToBlock("100/010/010/001/001"));    
        convertTable.Add('>', textToBlock("100/010/001/010/100"));    
        convertTable.Add('<', textToBlock("001/010/100/010/001"));    
        convertTable.Add('\'', textToBlock("010/010/000/000/000"));    
        convertTable.Add('(', textToBlock("001/010/010/010/001"));    
        convertTable.Add(')', textToBlock("100/010/010/010/100"));    
        convertTable.Add(';', textToBlock("000/010/000/010/100"));    
        convertTable.Add('=', textToBlock("000/111/000/111/000"));    
        convertTable.Add('[', textToBlock("011/010/010/010/011"));    
        convertTable.Add(']', textToBlock("110/010/010/010/110"));    
        convertTable.Add('{', textToBlock("011/010/100/010/011"));    
        convertTable.Add('}', textToBlock("110/010/001/010/110"));    
        convertTable.Add('^', textToBlock("010/101/000/000/000"));   
   
        colorTable.Add("W", "\uE006");      // white   
        colorTable.Add("G1", "\uE00E");      // light gray   
        colorTable.Add("G2", "\uE00D");     // darker gray   
        colorTable.Add("G3", "\uE009");     // even darker gray   
        colorTable.Add("G4", "\uE00F");     // even darker gray   
        colorTable.Add("G5", "!|!|!");            // even darker gray   
        colorTable.Add("G6", "'   '");            // darkest gray   
        colorTable.Add("G", "\uE001");        // green   
        colorTable.Add("B", "\uE002");       // blue   
        colorTable.Add("R", "\uE003");       // red   
        colorTable.Add("Y", "\uE004");       // yellow   
    }   
   
    public void Process() {  
        tick++;  
        if (tick >= updateTick) {  
            Show();  
            tick = 0;  
        }  
    }   
   
    public void RefreshData() {   
        if (ASCS_ind == 0)    
            ResetData();   
   
        var keys = new List<int>(elementList.Keys);   
        var layerElements = new List<DisplayElement>();   
        if (ASCS_lst != null) layerElements = ASCS_lst;   
        else {   
   
            for (var i = 0; i < keys.Count; i++) {   
                if (!elementList[keys[i]].isVisible) continue;   
                var layer = elementList[keys[i]].eLayer;   
                var index = 0;   
                for (var el = 0; el < layerElements.Count; el++) {   
                    if (layer <= layerElements[el].eLayer) index = el+1;   
                    else break;   
                }   
                layerElements.Insert(index, elementList[i]);   
            }   
        }   
        var ASCS_mergeLogic = 0;   
        for (var i = ASCS_ind; i < layerElements.Count; i++) {   
            ASCS_mergeLogic = (MergeData(layerElements[i]) ? 1 : 2);   
            if (ASCS_mergeLogic==2) {break;}   
            ASCS_ind = i;   
        }   
        if (ASCS_mergeLogic==2) {   
            ASCS_lst = layerElements;   
            return;   
        }   
        ASCS_lst = null;   
        ASCS_ind = 0;   
    }   
    public void RefreshScreen(string strData = "") {   
        if (strData == "") strData = ConvertData();   
        if (panel != null) {  
            panel.WritePublicText(strData);  
        } else if (meDebug != null) meDebug.Echo("No valid text panel");   
    }   
   
    public void Show() {  
        if (panel != null) panel.SetValueFloat("FontSize", fontSize);  
        RefreshData();  
        if (ASCS_ind == 0) {   
            var toShow = ConvertData();  
            if (panel.GetPublicText() != toShow) RefreshScreen(toShow);   
            else if (meDebug != null) meDebug.Echo("Text already shown");   
        }  
    }   
   
    public void ResetData() {   
        bgrArray = new string[x];   
        var color = colorTable[bgrColor];    
        for (var i = 0; i < x; i++) {   
            bgrArray[i] = color;   
        }   
        var newData = new List<List<string>>();    
        for (var ty = 0; ty < y; ty++) {   
            newData.Add(new List<string>(bgrArray));   
        }   
        data = newData;   
    }   
   
    public string ConvertData(List<List<string>> dat = null) {   
        if (dat == null) dat = data;   
        StringBuilder holder = new StringBuilder();   
        for (var row = 0; row < dat.Count; row++) {   
            holder.Append(string.Join("", dat[row]));   
            holder.Append("\n");   
        }  
        string holder2 = holder.ToString().Substring(0, holder.Length-1);  
        return holder2;   
    }  
   
    bool MergeData(DisplayElement what) {   
        if (runtime.CurrentInstructionCount > 49800) return false;   
        var ccTable = what.colorConvertTable;   
        var iposx = what.posX;   
        var iposy = what.posY;   
        var dat = what.eData;   
   
        for (var iy = 0; iy < dat.Count; iy++) {   
            for (var ix = 0; ix < dat[iy].Count; ix++) {   
                if ((iy+iposy) < y && (iy+iposy) >= 0 && (ix+iposx) < x && (ix+iposx) >= 0) {   
                    var d = dat[iy][ix];   
                    if (d != 0) {   
                        data[iy+iposy][ix+iposx] = (ccTable.ContainsKey(d) && colorTable.ContainsKey(ccTable[d]) ?   
                                colorTable[ccTable[d]] : colorTable["W"]);   
                    }   
                }   
                if (runtime.CurrentInstructionCount > 49800) return false;   
            }   
        }   
        return true;   
    }   
   
    public List<List<int>> textToBlock(string inp) {     
        var blck = new List<List<int>>();     
        foreach (var line in inp.Split('/')) {     
            var tmp = new List<int>();     
            var sp = line.ToCharArray();     
            for (var cha = 0; cha < sp.Length; cha++) {     
                var charac = sp[cha];     
                var x = 0;     
                Int32.TryParse(charac.ToString(), out x);     
                tmp.Add(x);     
            }     
            blck.Add(tmp);     
        }     
        return blck;     
    }  
    public IMyTextPanel GetPanel() {  
        return panel;  
    }  
    public void SetPanel(IMyTextPanel pan) {  
        panel = pan;  
    }  
    string Multiply(string what, int cnt) {   
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
    public int AddElementRectangle(int[] pos, int[] size, int thickness, int layer=0, string frameColor="Y",   
             string fillColor="") {     
        var id = id_index;     
        elementList.Add(id,    
            new DisplayElementRectangle(meDebug, pos, size, thickness, layer, frameColor, fillColor)    
                    as DisplayElement);     
        id_index++;     
        return id;     
    }   
    public int AddElementCircle(int[] pos, int[] size, int thickness, int layer=0, string frameColor="Y",    
             string fillColor="") {   
        var id = id_index;   
        elementList.Add(id,   
            new DisplayElementCircle(meDebug, pos, size, thickness, layer, frameColor, fillColor)   
                    as DisplayElement);   
        id_index++;   
        return id;   
    }   
    public int AddElementCustom(int[] pos, string data, Dictionary<int, string> cTable, int layer=0) {   
        var id = id_index;   
        elementList.Add(id,   
            new DisplayElementCustom(meDebug, pos, data, cTable, layer) as DisplayElement);   
        id_index++;   
        return id;   
    }   
    public void RemoveElement(int uid) {    
        DisplayElement holder = null;    
        if (elementList.ContainsKey(uid)) holder = elementList[uid];    
        elementList.Remove(uid);            // Hope Garbage Collector will work!   
    }   
    public DisplayElement GetElement(int uid) {   
        DisplayElement holder = null;   
        if (elementList.ContainsKey(uid)) holder = elementList[uid];   
        return holder;   
    }   
   
    public class DisplayElement {   
        public List<List<int>> eData;   
        public int posX;   
        public int posY;   
        public int eLayer;   
        public string eType;   
        public bool isVisible = true;   
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
        public void Update(string text="", string textColor="", string backColor=null) {   
            if (textColor!="") tColor = textColor;   
            if (backColor!=null) bColor = backColor;   
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
            colorConvertTable = new Dictionary<int, string>() {{1, pColor},{2, fColor},{3, pColor2},{4, pColor3}};   
   
            eData = CreateProgressBar(progress);   
        }   
        public override void Refresh() {    
            eData=CreateProgressBar(progress);    
        }   
        public float GetProgress() {   
            return progress;   
        }   
    }   
    public class DisplayElementRectangle : DisplayElement {   
        public int sizeX;   
        public int sizeY;   
        public int thick;   
        public string fColor;   
        public string inColor;   
   
        public DisplayElementRectangle(MyGridProgram m, int[] position, int[] size, int thickness = 1, int layer = 0,   
                        string frameColor="Y", string fillColor = "")   
                                : base(m, "RECT", position, layer) {   
            sizeX = size[0];   
            sizeY = size[1];   
            thick = thickness;   
            fColor = frameColor;   
            inColor = fillColor;   
            colorConvertTable = new Dictionary<int, string>() {{1, fColor},{2, inColor}};   
   
            eData = DrawRectangle();   
        }   
        public List<List<int>> DrawRectangle() {   
            List<List<int>> lst = new List<List<int>>();   
            var borderLine = new int[sizeX];    
            var body = new int[sizeX];    
            for (var i = 0; i < sizeX; i++) {    
                borderLine[i] = 1;   
                body[i] = ((i >= thick && i < sizeX-thick) ? (inColor != "" ? 2 : 0) : 1);    
            }   
            for (var iy = 0; iy < sizeY; iy++) {   
                if (iy<thick || iy>=sizeY-thick) {   
                    lst.Add(new List<int>(borderLine));   
                    continue;   
                } else lst.Add(new List<int>(body));   
            }   
            return lst;   
        }   
        public void Update(int sizex=-1, int sizey=-1,int thickness=-1, string frameColor="", string fillColor=null) {   
            if (sizex!=-1) sizeX = sizex;   
            if (sizey!=-1) sizeY = sizey;   
            if (thickness!=-1) thick = thickness;   
            if (frameColor!="") fColor = frameColor;   
            if (fillColor!=null) inColor = fillColor;   
            colorConvertTable = new Dictionary<int, string>() {{1, fColor},{2, inColor}};   
   
            eData = DrawRectangle();   
        }   
        public override void Refresh() {   
            eData = DrawRectangle();   
        }   
    }   
    class DisplayElementCircle : DisplayElement {   
        int sizeX;   
        int sizeY;   
        int thick;   
        string fColor;   
        string inColor;   
           
        public DisplayElementCircle(MyGridProgram m, int[] position, int[] size, int thickness=1, int layer=0,   
                string color = "Y", string fillColor = "") : base(m, "CIRCLE", position, layer) {   
            thick = thickness;   
            sizeX = size[0];   
            sizeY = size[1];   
            fColor = color;   
            inColor = fillColor;   
            colorConvertTable = new Dictionary<int, string>() {{1, fColor},{2, inColor}};   
   
            eData = DrawCircle();   
        }   
        public List<List<int>> DrawCircle() {   
            List<List<int>> lst = new List<List<int>>();    
            var center = new float[] {sizeX/2f, sizeY/2f};   
            float a2 = (float)Math.Pow(sizeX/2f,2);   
            float b2 = (float)Math.Pow(sizeY/2f,2);   
            for (var iy = 0.5f; iy < sizeY; iy++) {   
                lst.Add(new List<int>());   
                for (var ix = 0.5f; ix < sizeX; ix++) {   
                    double length = (Math.Pow(ix-center[0], 2)/a2 + Math.Pow(iy-center[1], 2)/b2);   
                    double length_min = (Math.Pow(ix-center[0], 2)/Math.Pow(sizeX/2f-thick,2) +   
                        Math.Pow(iy-center[1], 2)/Math.Pow(sizeY/2f-thick,2));   
                    lst[(int)iy].Add(length < 1 ? (length_min >= 1 ? 1 : (inColor == "" ? 0 : 2)) : 0);   
                }   
            }   
            return lst;   
        }   
        public void Update(int sizex=-1, int sizey=-1,int thickness=-1, string frameColor="", string fillColor=null) {    
            if (sizex!=-1) sizeX = sizex;    
            if (sizey!=-1) sizeY = sizey;    
            if (thickness!=-1) thick = thickness;    
            if (frameColor!="") fColor = frameColor;    
            if (fillColor!=null) inColor = fillColor;   
            colorConvertTable = new Dictionary<int, string>() {{1, fColor},{2, inColor}};   
    
            eData = DrawCircle();    
        }   
        public override void Refresh() {    
            eData = DrawCircle();    
        }   
    }   
    class DisplayElementCustom : DisplayElement {   
   
        public DisplayElementCustom(MyGridProgram m, int[] position, string data, Dictionary<int, string> colTable,   
                        int layer=0 )   
                    : base(m, "CUSTOM", position, layer) {   
            colorConvertTable = colTable;   
            eData = ConvertCustomData(data);   
        }   
        List<List<int>> ConvertCustomData(string dat) {   
            var lst = new List<List<int>>();   
            var datArr = dat.Split('\n');   
            for (var i = 0; i < datArr.Length; i++) {   
                lst.Add(new List<int>());   
                for (var a = 0; a < datArr[i].Length; a++) {   
                    var holder = 0;   
                    Int32.TryParse(datArr[i][a].ToString(), out holder);   
                    lst[i].Add(holder);   
                }   
            }   
            return lst;   
        }   
        public void Update(string data=null, Dictionary<int, string> colorTable=null, List<List<int>> dataRaw=null) {   
            if (data != null) eData = ConvertCustomData(data);   
            if (colorTable != null) colorConvertTable = colorTable;   
            if (dataRaw!=null) eData = dataRaw;   
        }   
    }   
}