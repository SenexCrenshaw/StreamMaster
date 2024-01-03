namespace StreamMaster.Domain.Extensions;

public static class ListComparer
{
    public static bool AreListsEqual(this List<string> list1, List<string> list2)
    {
        return list1 == null || list2 == null ? list1 == list2 : list1.Count == list2.Count && !list1.Except(list2).Any();
    }
}
