using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var input = lines[0];

var len = input.Select(c => (long)char.GetNumericValue(c)).Sum();
var (files, spaces) = GetSections(input);
var decoded = Decode(files, len);

int?[] ordered = decoded.ToArray();
var remainingFreeSpaces1 = spaces.Select(s => new Section(s.Index, s.Size)).ToList();
for (int i = decoded.Length - 1; i >= 0; i--)
{
    var id = decoded[i];
    if (id != null)
    {
        var freeSpace = remainingFreeSpaces1.FirstOrDefault(space => space.Size > 0);
        if (freeSpace == default || freeSpace.Index > i)
        {
            break;
        }
        ordered[freeSpace.Index] = id;
        ordered[i] = null;
        freeSpace.Index++;
        freeSpace.Size--;
    }
}
var checksum1 = ordered.TakeWhile(id => id != null).Select((v, i) => (long)i * v).Sum();
Console.WriteLine(checksum1);

int?[] ordered2 = decoded.ToArray();
var remainingFreeSpaces2 = spaces.Select(s => new Section(s.Index, s.Size)).ToList();
for (int id = files.Count - 1; id > 0; id--)
{
    var file = files[id];
    var freeSpace = remainingFreeSpaces2.FirstOrDefault(space => space.Size >= file.Size && space.Index < file.Index);
    if (freeSpace == default)
    {
        continue;
    }
    
    for (int j = 0; j < file.Size; j++)
    {
        ordered2[freeSpace.Index + j] = id;
        ordered2[file.Index + j] = null;
    }
    freeSpace.Index += file.Size;
    freeSpace.Size -= file.Size;
}

var checksum2 = ordered2.Select((v, i) => (long)i * v ?? 0).Sum();
Console.WriteLine(checksum2);

Console.WriteLine("done");

(List<Section> Files, List<Section> Spaces) GetSections(string fileSystem)
{
    List<Section> fileSections = new();
    List<Section> spaceSections = new();
    int index = 0;
    for (int i = 0; i < fileSystem.Length; i++)
    {
        var number = fileSystem[i].ToInt();
        if (i % 2 == 0)
        {
            fileSections.Add(new (index, number));
        }
        else
        {
            spaceSections.Add(new (index, number));
        }
        index += number;
    }
    
    return (fileSections, spaceSections);
}

int?[] Decode(List<Section> fileSections, long size)
{
    int?[] result = new int?[size];
    for (int fileId = 0; fileId < fileSections.Count; fileId++)
    {
        var file = fileSections[fileId];
        for (int j = 0; j < file.Size; j++)
        {
            result[file.Index + j] =  fileId;
        }
    }

    return result;
}

class Section(int index, int size)
{
    public int Index { get; set; } = index;
    public int Size { get; set; } = size;
}