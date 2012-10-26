using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {

    public class Entity {

        /// <summary>
        /// 
        /// </summary>
        public int Start { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int End { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// ListSlug is used to store the list portion of @mention/list. 
        /// </summary>
        public string ListSlug { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public EntityType Type { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string DisplayURL { get; set;}
        
        /// <summary>
        /// 
        /// </summary>
        public string ExpandedURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="listSlug"></param>
        /// <param name="type"></param>
        public Entity(int start, int end, string value, string listSlug, EntityType type) {
            Start = start;
            End = end;
            Value = value;
            ListSlug = listSlug;
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public Entity(int start, int end, string value, EntityType type)
            : this(start, end, value, null, type) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="type"></param>
        /// <param name="groupNumber"></param>
        public Entity(System.Text.RegularExpressions.Match matcher, EntityType type, int groupNumber)
            : this(matcher, type, groupNumber, -1) { // Offset -1 on start index to include @, # symbols for mentions and hashtags

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="type"></param>
        /// <param name="groupNumber"></param>
        /// <param name="startOffset"></param>
        public Entity(System.Text.RegularExpressions.Match matcher, EntityType type, int groupNumber, int startOffset) :
            this(matcher.Groups[groupNumber].Index + startOffset, matcher.Groups[groupNumber].Index + matcher.Groups[groupNumber].Length, matcher.Groups[groupNumber].Value, type) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }

            if (!(obj is Entity)) {
                return false;
            }

            Entity other = (Entity)obj;

            if (Type.Equals(other.Type) &&
                Start == other.Start &&
                End == other.End &&
                Value.Equals(other.Value)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return Type.GetHashCode() + Value.GetHashCode() + Start + End;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Value + "(" + Type + ") [" + Start + "," + End + "]";
        }

    }
}

