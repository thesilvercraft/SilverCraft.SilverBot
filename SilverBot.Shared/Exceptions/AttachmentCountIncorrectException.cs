/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using SilverBotDS.Converters;
using System.Runtime.Serialization;

namespace SilverBotDS.Exceptions;

[Serializable]
public class AttachmentCountIncorrectException : Exception
{
    public AttachmentCountIncorrectException(AttachmentCountIncorrect count)
    {
        SetAttachmentCount(count);
    }

    public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message) : base(message)
    {
        SetAttachmentCount(count);
    }

    public AttachmentCountIncorrectException(AttachmentCountIncorrect count, string message, Exception inner) : base(
        message, inner)
    {
        SetAttachmentCount(count);
    }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected AttachmentCountIncorrectException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public AttachmentCountIncorrect AttachmentCount { get; private set; }

    public void SetAttachmentCount(AttachmentCountIncorrect value)
    {
        AttachmentCount = value;
    }
}