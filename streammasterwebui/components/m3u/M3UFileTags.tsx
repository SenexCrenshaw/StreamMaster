import DownArrowButton from '@components/buttons/DownArrowButton';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { Chips } from 'primereact/chips';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { useMemo, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

export interface M3UFileTagsProperties {
  m3uFileDto?: M3UFileDto;
  onChange: (vodTags: string[]) => void;
  vodTags?: string[];
}

const M3UFileTags = ({ m3uFileDto, onChange, vodTags }: M3UFileTagsProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorReference = useRef(null);
  const uuid = uuidv4();
  const [isOpen, setIsOpen] = useState(false);

  const intTags = useMemo((): string[] => {
    const tags = m3uFileDto ? m3uFileDto.VODTags : vodTags ?? [];

    return tags;
  }, [m3uFileDto, vodTags]);

  const buttonTags = useMemo((): string => {
    if (intTags.length > 0 && intTags.length < 3) return intTags.join(', ');

    return intTags.length + ' Tags';
  }, [intTags]);

  return (
    <div className="col-12 p-0 pb-4">
      <div className="text-xs">
        <label className="flex p-0 text-500 w-full justify-content-center align-items-center" htmlFor={uuid}>
          URL REGEX
        </label>
        <div id={uuid} className="w-full tag-editor p-0 m-0 input-border" ref={anchorReference}>
          <OverlayPanel ref={op} onHide={() => setIsOpen(false)} className="w-2">
            <div className="flex w-full">
              <Chips
                className="w-full border-1"
                autoFocus
                id="chips"
                value={intTags}
                onChange={(e) => {
                  console.log('change', e.value);
                  onChange(e.value ?? []);
                }}
              />
            </div>
          </OverlayPanel>

          <div>
            <DownArrowButton
              label={buttonTags}
              tooltip=""
              onClick={(e) => {
                if (isOpen) {
                  op.current?.hide();
                } else {
                  op.current?.show(null, anchorReference.current);
                }
                setIsOpen(!isOpen);
              }}
            />
            {/* <Button
            className="text-sm w-full"
            icon="pi pi-chevron-down"
            iconPos="right"
            id="tag-Button"
            label={buttonTags}
            text={true}
            onClick={(e) => {
              if (isOpen) {
                op.current?.hide();
              } else {
                op.current?.show(null, anchorReference.current);
              }
              setIsOpen(!isOpen);
            }}
          /> */}
          </div>
        </div>
      </div>
    </div>
  );
};

M3UFileTags.displayName = 'M3UFileDialog';

export default React.memo(M3UFileTags);
