namespace Imagegram.Api.Models.Entity
{
    public interface ICursoredModel
    {
        long VersionCursor { get; set; }
    }
}