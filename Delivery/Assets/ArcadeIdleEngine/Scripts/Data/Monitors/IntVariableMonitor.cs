using ArcadeBridge.ArcadeIdleEngine.Data.Variables;
using TMPro;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Data.Monitors
{
	public class IntVariableMonitor : MonoBehaviour
	{
		[SerializeField] IntVariable _monitorVariable;
		[SerializeField] TextMeshProUGUI _monitorText;

		void OnEnable()
		{
			_monitorVariable.ValueSet += SetText;
		}
		
		void OnDisable()
		{
			_monitorVariable.ValueSet -= SetText;
		}

		void Start()
		{
			if(SaveLoadService.instance != null)
				_monitorVariable.RuntimeValue = SaveLoadService.instance.PlayerProgress.money;
            else
				SetText(_monitorVariable.RuntimeValue);
		}

		void SetText(int obj)
		{
			_monitorText.text = obj.ToString();
			if (SaveLoadService.instance != null)
			{
				SaveLoadService.instance.PlayerProgress.money = obj;
				//SaveLoadService.instance.PlayerProgress.money += 150;
				SaveLoadService.instance.DelayedSaveProgress();
			}
			//_monitorText.text = obj.ToString();
		}
	}
}
