//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/packet.proto
namespace com.crownrace.msg
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"packet")]
  public partial class packet : global::ProtoBuf.IExtensible
  {
    public packet() {}
    
    private com.crownrace.msg.NET_CMD _cmd;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"cmd", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public com.crownrace.msg.NET_CMD cmd
    {
      get { return _cmd; }
      set { _cmd = value; }
    }
    private byte[] _payload;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"payload", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] payload
    {
      get { return _payload; }
      set { _payload = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"NET_CMD")]
    public enum NET_CMD
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_REQ_CMD", Value=1)]
      LOGIN_REQ_CMD = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_ACK_CMD", Value=2)]
      LOGIN_ACK_CMD = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"HEARTBEAT_REQ_CMD", Value=3)]
      HEARTBEAT_REQ_CMD = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"HEARTBEAT_ACK_CMD", Value=4)]
      HEARTBEAT_ACK_CMD = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LEAVE_GAME_NTF_CMD", Value=5)]
      LEAVE_GAME_NTF_CMD = 5
    }
  
}