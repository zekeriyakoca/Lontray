using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Dtos
{
  public class QueueNameEnum
  {
    private QueueNameEnum(string @value)
    {
      this.Value = value;
    }
    public string Value { get; set; }

    public static QueueNameEnum ImageToCompress { get { return new QueueNameEnum(nameof(ImageToCompress)); } }
    public static QueueNameEnum ImageCompressed { get { return new QueueNameEnum(nameof(ImageCompressed)); } }
    //public static QueueNameEnum MailSent { get { return new QueueNameEnum(nameof(MailSent)); } }
  }
}
