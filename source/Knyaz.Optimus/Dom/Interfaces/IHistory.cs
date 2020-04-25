
namespace Knyaz.Optimus.Dom.Interfaces
{
	public interface IHistory
	{
		long Length { get; }
		void Go(long delta = 0);
		void Back();
		void Forward();
		void ReplaceState(object data, string title, string url);
		void PushState(object data, string title, string url);
	}
}
