namespace vm.component
{
    public interface IHalter
    {
        int halt(int reason, string text = "");
    }
}