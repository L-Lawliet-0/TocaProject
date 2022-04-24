using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class AttachmentControl
{
    private string SlotName; // the slot name that control the attachments
    private Skeleton MySkeleton;
    private string Prefix;
    public AttachmentControl(string slotName, Skeleton skeleton, string prefix)
    {
        MySkeleton = skeleton;
        SlotName = slotName;
        Prefix = prefix;
        SetAttachment(); // set to none
    }

    // if the incoming parameter is empty, set attachment to none
    public void SetAttachment(int id = -1)
    {
        if (id == -1)
            MySkeleton.SetAttachment(SlotName, null);
        else
            MySkeleton.SetAttachment(SlotName, Prefix + id);
    }

    public void SetAttachment(string name)
    {
        MySkeleton.SetAttachment(SlotName, name);
    }
}
