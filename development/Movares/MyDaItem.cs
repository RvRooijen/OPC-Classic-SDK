using Softing.OPCToolbox;
using Softing.OPCToolbox.Client;

namespace Movares
{
    public class MyDaItem : DaItem
    {
        public string ItemId { get; private set; }
        public string ItemPath { get; private set; }
        public ValueQT Value { get; private set; }

        public MyDaItem(string itemId, MyDaSubscription parentSubscription) : base(itemId, parentSubscription)
		{
            ValueChanged += new ValueChangedEventHandler(HandleValueChanged);
			StateChangeCompleted += new StateChangeEventHandler(HandleStateChanged);
			PerformStateTransitionCompleted += new PerformStateTransitionEventHandler(HandlePerformStateTransition);
        }

       public static void HandleStateChanged(ObjectSpaceElement sender, EnumObjectState state)
		{
			MyDaItem item = sender as MyDaItem;
			System.Console.WriteLine(item + " " + item.Id + " State Changed - " + state);
		}


		public static void HandleValueChanged(DaItem aDaItem, ValueQT aValue)
		{
				System.Console.WriteLine("Value changed!");
				System.Console.WriteLine(String.Format("{0,-19} {1} {2,-50} ", aDaItem.Id, "-", aValue.ToString()));
		}


		public static void HandlePerformStateTransition(
			ObjectSpaceElement sender,
			uint executionContext,
			int result)
		{
			if (ResultCode.SUCCEEDED(result))
			{
				MyDaItem item = sender as MyDaItem;
				System.Console.WriteLine(sender + " " + item.Id + " Performed state transition - " + executionContext);
			}
			else
			{
				System.Console.WriteLine(sender + "  Performed state transition failed! Result: " + result);
			}
		}
    }
}
