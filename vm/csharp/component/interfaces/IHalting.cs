namespace vm.component
{
    public interface IHalting
    {
        int halt(int reason, string text = "");
    }
}