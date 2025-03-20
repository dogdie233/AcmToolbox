using AcmToolbox;

using static AcmToolbox.ConsoleWriter;

using var io = new IO();

var n = 0;
var meow = "owo";

io.writer.Write($"{Endl}n is {n}{Endl}meow is {meow}{Endl}");
io.writer.Write($"到你输入了：");
io.reader.Read($"{n}{meow}");
io.writer.Write($"{Endl}n is {n}{Endl}meow is {meow}{Endl}");