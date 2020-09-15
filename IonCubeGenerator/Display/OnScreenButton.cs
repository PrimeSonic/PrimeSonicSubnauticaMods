namespace IonCubeGenerator.Display
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Component that buttons on the power storage ui will inherit from. Handles working on whether something is hovered via IsHovered as well as interaction text. 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    internal abstract class OnScreenButton : MonoBehaviour
    {
        protected bool IsHovered { get; set; }
        public string TextLineOne { get; set; }
        public string TextLineTwo { get; set; }
        private bool isHoveredOutOfRange;

        public virtual void OnDisable()
        {
            this.IsHovered = false;
            isHoveredOutOfRange = false;
        }

        public virtual void Update()
        {
            bool inInteractionRange = InInteractionRange();

            if (this.IsHovered && inInteractionRange)
            {
#if SUBNAUTICA
                HandReticle.main.SetInteractTextRaw(this.TextLineOne, this.TextLineTwo);
#elif BELOWZERO
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, this.TextLineOne);
                HandReticle.main.SetTextRaw(HandReticle.TextType.HandSubscript, this.TextLineTwo);
#endif
            }

            if (this.IsHovered && inInteractionRange == false)
            {
                this.IsHovered = false;
            }

            if (this.IsHovered == false && isHoveredOutOfRange && inInteractionRange)
            {
                this.IsHovered = true;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (InInteractionRange())
            {
                this.IsHovered = true;
            }

            isHoveredOutOfRange = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.IsHovered = false;
            isHoveredOutOfRange = false;
        }

        protected bool InInteractionRange()
        {
            return Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, Player.main.transform.position)) <= 2.5;
        }
    }
}
