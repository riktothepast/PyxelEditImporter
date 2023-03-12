using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace net.fiveotwo.pyxelImporter
{
    public class CreateAnimation
    {
        public CreateAnimation(DocData doc, List<Sprite> animationFrames, string path)
        {
            foreach (int key in doc.animations.Keys)
            {
                doc.animations.TryGetValue(key, out Animation animation);
                List<Sprite> currentAnimationFrames = new ();
                foreach (Sprite sprite in animationFrames)
                {
                    if (sprite.name.Contains(animation.name))
                    {
                        currentAnimationFrames.Add(sprite);
                    }
                    AnimationClip clip = new();
                    clip.frameRate = animation.frameDuration * animation.length;

                    EditorCurveBinding spriteBinding = new();
                    spriteBinding.type = typeof(SpriteRenderer);
                    spriteBinding.path = "";
                    spriteBinding.propertyName = "m_Sprite";

                    ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[currentAnimationFrames.Count];
                    for (int index = 0; index < spriteKeyFrames.Length; index++)
                    {
                        spriteKeyFrames[index] = new ObjectReferenceKeyframe
                        {
                            time = index / clip.frameRate,
                            value = currentAnimationFrames[index]
                        };
                    }
                    AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

                    AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(clip, settings);

                    AssetDatabase.CreateAsset(clip, $"{HelperClass.GetRelativePath(path)}/{animation.name}.anim");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}