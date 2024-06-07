import SMOverlay from '@components/sm/SMOverlay';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { Chips } from 'primereact/chips';
import React, { useMemo, useRef } from 'react';
import { v4 as uuidv4 } from 'uuid';

export interface M3UFileTagsProperties {
  m3uFileDto?: M3UFileDto;
  onChange: (vodTags: string[]) => void;
  vodTags?: string[];
}

const M3UFileTags = ({ m3uFileDto, onChange, vodTags }: M3UFileTagsProperties) => {
  const anchorReference = useRef(null);
  const uuid = uuidv4();

  const intTags = useMemo((): string[] => {
    const tags = m3uFileDto ? m3uFileDto.VODTags : vodTags ?? [];

    return tags;
  }, [m3uFileDto, vodTags]);

  const buttonTags = useMemo((): string => {
    if (intTags.length > 0 && intTags.length < 3) return intTags.join(', ');

    return intTags.length + ' Tags';
  }, [intTags]);

  const buttonTemplate = useMemo(() => {
    return <div className="text-container pl-1">{buttonTags}</div>;
  }, [buttonTags]);

  return (
    <div className="sm-w-12 ">
      <label className="flex text-xs text-default-color w-full justify-content-start align-items-center pl-2" htmlFor={uuid}>
        URL REGEX
      </label>
      <div id={uuid} className="w-full tag-editor p-0 m-0 pt-small" ref={anchorReference}>
        <SMOverlay
          buttonDarkBackground
          buttonTemplate={buttonTemplate}
          title="URL Regex Tags"
          contentWidthSize="3"
          icon="pi-chevron-down"
          buttonLabel="GROUP"
          header={<></>}
        >
          <div className="h-full p-fluid">
            <Chips
              autoFocus
              value={intTags}
              onChange={(e) => {
                onChange(e.value ?? []);
              }}
              variant="filled"
            />
          </div>
        </SMOverlay>

        {/* <div className="flex w-full">
          <DownArrowButton
            className="w-full"
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
        </div> */}
      </div>
    </div>
  );
};

M3UFileTags.displayName = 'M3UFileTags';

export default React.memo(M3UFileTags);
