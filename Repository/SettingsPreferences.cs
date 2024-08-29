namespace Tsundoku.Repository;

public class SettingsPreferences
{
    private string _isSubscribed = "IsSubscribed";
    private IPreferences _defaultPreferences;
    public SettingsPreferences(IPreferences defaultPreferences)
    {
        _defaultPreferences = defaultPreferences;
    }
    
    public void SetIsSubscribed(bool isSubscribed)
    {
        _defaultPreferences.Set(_isSubscribed, isSubscribed.ToString());
    }

    public bool GetIsSubscribed()
    {
        return bool.Parse(_defaultPreferences.Get(_isSubscribed, false.ToString()));
    }
}
