using System.Runtime.CompilerServices;
using Xunit.Abstractions;

public class SymbolTableGen : IVisitor
{
    private string _outputFile;
    private string _content; 
    public SymbolTableGen(string output){
        _outputFile = output;
        _content = "";
    }

    public void Visit(ImplNode node)
    {
        SymbolTable upper = node._symbolTable;
        string implName = node._LeftMostchild._value;
        string type = "IMPL";
        int level = node._symbolTable._tablelevel +1;
        List<FunctionEntry> functions = new();
        node._symbolTable = new SymbolTable(level,implName,upper);

        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
           child.Accept(this); 
        }

    }

    public void Visit(StructNode node)
    {
        String className = node._LeftMostchild._value;
        int tableLevel = 1;
        SymbolTable upper = node._symbolTable;

        //adding the entry for the GLOBAL TABLE which is the table initially
        var structTableEntry = new ClassEntry(className,node._symbolTable);
        node._symbolTable.addEntry(structTableEntry);

        //creating the symbol table for this class and reassign it to itself instead of global
        SymbolTable structTable = new SymbolTable(tableLevel, className,upper);
        node._symbolTable = structTable;
        node._symtabentry = structTableEntry;

        //every child gets assigned the same table because members of the class
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
        _content += node._symbolTable;
        //Console.WriteLine(node._symbolTable);
        
    }

    public void Visit(FuncDefNode node){
        string fName = node._LeftMostchild._LeftMostchild._value;
        string fReturnType = node._LeftMostchild._LeftMostchild._rightSibling._value;
        List<VarEntry> functionParams = new();
        SymbolTable upper = node._symbolTable;
        int tableLevel = node._symbolTable._tablelevel + 1;

        var funcDefEntry = new FunctionEntry(fReturnType,fName,functionParams,upper)  ;

        node._symbolTable.addEntry(funcDefEntry);
        SymbolTable thisFunction = new(tableLevel, upper._title + "::"  + fName , upper);

        node._symbolTable = thisFunction;
        node._symtabentry = funcDefEntry;

        //Let's now deal with the local variables inside the function

        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
        _content += node._symbolTable;

        //Console.WriteLine(thisFunction);

    }

    

    public void Visit(ProgNode progNode)
    {
        SymbolTable globalTable = new(0,"Global",null);
        progNode._symbolTable = globalTable;
        foreach(var child in progNode.getChildren()){

            child._symbolTable = progNode._symbolTable;
			child.Accept(this);
        }
        _content += progNode._symbolTable;
        //Console.WriteLine(_content);
    }

    public void Visit(Node node)
    {
            foreach (var child in node.getChildren())
            {
                child._symbolTable = node._symbolTable;
                child.Accept(this);
            }
			
    }

    public void Visit(IdNode node)
    {
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
           child.Accept(this); 
        }
    }

    //Function Declaration
    public void Visit(FuncDeclNode node)
    {   
        string fName = node._LeftMostchild._value;
        string fReturnType = node.GetRightMostChild()._value;
        int localLevel = node._symbolTable._tablelevel+ 1;


        SymbolTable local = new(localLevel,fName,node._symbolTable);
        List<VarEntry> fParam = new();

        if (node._LeftMostchild._rightSibling._type == NodeType.FPARAMS)
        {
            var FuncParamsNode = node._LeftMostchild._rightSibling;
            var listOfP = FuncParamsNode.getChildren();

            for (int i = 0; i < listOfP.Count; i++)
            {
                var variableEntry = new VarEntry();
                variableEntry._kind = "Func Parameter";
                if(listOfP[i]._type == NodeType.ID){
                    variableEntry._name = listOfP[i]._value;
                    i++;
                }
                if (listOfP[i]._type == NodeType.TYPE)
                {
                    variableEntry._type = listOfP[i]._value;
                    i++;
                }
                fParam.Add(variableEntry);
            }
        }

        
        FunctionEntry funcEntry = new FunctionEntry(fReturnType,fName,fParam,node._symbolTable);
        funcEntry._kind = "Function Declaration";
        node._symtabentry = funcEntry;
        node._symbolTable.addEntry(funcEntry);
        node._symbolTable = local;

        foreach(var child in node.getChildren()){
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
        
       
    }

    public void Visit(VarDeclNode node)
    {
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }

        string variableId = node._LeftMostchild._value;
        string varaibeleType = node._LeftMostchild._rightSibling._value;

        List<int> dimensions = new List<int>();

        node._symtabentry = new VarEntry("Var",varaibeleType,variableId,dimensions);
        node._symbolTable.addEntry(node._symtabentry);
    }

    public void Visit(AddNode node)
    {
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
    }

    public void Visit(MultNode node)
    {
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
    }

    public void Visit(IntlitNode node)
    {
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
    }

    public void Visit(FloatLitNode node)
    {
        foreach (var child in node.getChildren())
        {
            child._symbolTable = node._symbolTable;
            child.Accept(this);
        }
    }

    public void write(){
        try
        {
            using var writer = new StreamWriter(_outputFile);
            writer.Write(_content);
            writer.Close();

        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {_outputFile}");
            throw;
        }
    }
}