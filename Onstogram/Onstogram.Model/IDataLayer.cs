using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onstogram.Model
{
    public interface IDataLayer
    {
        User AddUser(User user);
        User GetUser(Guid id);

        Image AddImage(Image image);
        Image GetImage(Guid imageId);
        Image DeleteImage(Guid imageId);

        Comment AddCommentToImage(Comment comment, Guid imageId);
        Comment[] GetImageComments(Guid imageId);
        Comment DeleteComment(Guid commentId);

        Image[] GetLastImages();
    }
}
