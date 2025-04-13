namespace DotNetBucketry.Core.Communication.Files;

public class AddFileResponse
{
    public IList<string> PreSignedUrls { get; set; }= new List<string>();
}