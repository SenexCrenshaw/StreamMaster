import { Dropdown } from 'primereact/dropdown';
import React, { useCallback, useRef } from 'react';

interface SMSideCardProps {
  anchorRef: React.RefObject<Dropdown>;
  readonly children: React.ReactNode;
  readonly direction?: 'left' | 'right';
}

const SMSideCard: React.FC<SMSideCardProps> = ({ anchorRef, children, direction }) => {
  const popupRef = useRef<HTMLDivElement>(null);

  const calculatePopupPosition = useCallback(
    (popupRef2: React.RefObject<HTMLDivElement>) => {
      if (anchorRef.current && popupRef2.current) {
        const anchorRect = anchorRef.current.getElement().getBoundingClientRect();

        const popupRect = popupRef2.current.getBoundingClientRect();

        let height = anchorRect.height;
        const panel = document.querySelector('.sm-epg-editor-panel');
        if (panel) {
          var p = panel.getBoundingClientRect();
          height = p.height;
        }

        if (direction === 'right') {
          return { height: height, left: popupRect.width * 2 };
        }

        return { height: height, left: -popupRect.width };
      }
      return { height: 0, left: 0 };
    },
    [anchorRef, direction]
  );

  return (
    <div
      className="w-4 surface-card sm-input-border sm-SideCard"
      ref={popupRef}
      style={{
        height: calculatePopupPosition(popupRef).height,
        left: calculatePopupPosition(popupRef).left,
        position: 'absolute',
        top: -2
      }}
    >
      {children}
    </div>
  );
};

export default SMSideCard;
