using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterText {

    public class Entity {

        internal int start;
        internal int end;
        internal readonly string value;
        // listSlug is used to store the list portion of @mention/list.
        internal readonly string listSlug;
        internal readonly EntityType type;

        internal string displayURL = null;
        internal string expandedURL = null;

        public Entity(int start, int end, string value, string listSlug, EntityType type) {
            this.start = start;
            this.end = end;
            this.value = value;
            this.listSlug = listSlug;
            this.type = type;
        }

        public Entity(int start, int end, string value, EntityType type)
            : this(start, end, value, null, type) {
        }

        // Offset -1 on start index to include @, # symbols for mentions and hashtags
        public Entity(System.Text.RegularExpressions.Match matcher, EntityType type, int groupNumber)
            : this(matcher, type, groupNumber, -1) {
        }

        public Entity(System.Text.RegularExpressions.Match matcher, EntityType type, int groupNumber, int startOffset) :
            this(matcher.Groups[groupNumber].Index + startOffset, matcher.Groups[groupNumber].Index + matcher.Groups[groupNumber].Length, matcher.Groups[groupNumber].Value, type) {
        }

        // TODO: change to public override bool Equals()
        public bool equals(Object obj) {
            if (this == obj) {
                return true;
            }

            if (!(obj is Entity)) {
                return false;
            }

            Entity other = (Entity)obj;

            if (this.type.Equals(other.type) &&
                this.start == other.start &&
                this.end == other.end &&
                this.value.Equals(other.value)) {
                return true;
            } else {
                return false;
            }
        }


        // TODO: change to public override int GetHashCode()
        public int hashCode() {
            return this.type.GetHashCode() + this.value.GetHashCode() + this.start + this.end;
        }

        // TODO: change to public override int ToString()
        public string toString() {
            return value + "(" + type + ") [" + start + "," + end + "]";
        }

        public int getStart() {
            return start;
        }

        public int getEnd() {
            return end;
        }

        public string getValue() {
            return value;
        }

        public string getListSlug() {
            return listSlug;
        }

        public EntityType getType() {
            return type;
        }

        public string getDisplayURL() {
            return displayURL;
        }

        public void setDisplayURL(string displayURL) {
            this.displayURL = displayURL;
        }

        public string getExpandedURL() {
            return expandedURL;
        }

        public void setExpandedURL(string expandedURL) {
            this.expandedURL = expandedURL;
        }
    }
}

