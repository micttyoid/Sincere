namespace Sincere;

public class ViewerEventArgs
{
    public ViewerEventArgs(int currentPage, int totalPages)
    {
        CurrentPage = currentPage;
        TotalPages = totalPages;
    }
    public int CurrentPage { get; }

    public int TotalPages { get; }

}