using System.Collections.Generic;

public interface ILevelHandler
{
    List<string> GetExpectedFields(); // специфические поля
    void LoadFromDictionary(Dictionary<string, string> specific);
}
