import { Button } from 'primereact/button';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useRef, useState } from 'react';
import PlayListDataSelector from './PlayListDataSelector';

interface PlayListDataSelectorDropDownProperties {
  readonly id: string;
}

const PlayListDataSelectorDropDown = ({ id }: PlayListDataSelectorDropDownProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorReference = useRef(null);

  const [isOpen, setIsOpen] = useState(false);
  return (
    <div className="" ref={anchorReference}>
      <Button
        className="bordered"
        icon="pi pi-chevron-down"
        label="Channel Groups"
        text={true}
        onClick={(e) => {
          if (isOpen) {
            op.current?.hide();
          } else {
            op.current?.show(null, anchorReference.current);
          }
          setIsOpen(!isOpen);
        }}
      />
      <OverlayPanel ref={op} onHide={() => setIsOpen(false)}>
        <PlayListDataSelector id={id} />
      </OverlayPanel>
    </div>
  );
};

export default memo(PlayListDataSelectorDropDown);
