using System.Collections.Generic;

public interface ILevelHandler
{
    List<string> GetExpectedFields(); // ������������� ����
    void LoadFromDictionary(Dictionary<string, string> specific);
}
