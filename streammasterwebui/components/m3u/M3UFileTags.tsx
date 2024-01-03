import { M3UFileDto, useM3UFilesUpdateM3UFileMutation, type UpdateM3UFileRequest } from '@lib/iptvApi';
import { Button } from 'primereact/button';
import { Chips } from 'primereact/chips';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { useMemo, useRef, useState } from 'react';

export interface M3UFileTagsProperties {
  m3uFileDto: M3UFileDto;
}

const M3UFileTags = ({ m3uFileDto }: M3UFileTagsProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorReference = useRef(null);
  const [M3UFilesUpdateM3UFileMutation] = useM3UFilesUpdateM3UFileMutation();
  const [isOpen, setIsOpen] = useState(false);

  const updateM3U = async (vodTags: string[]) => {
    const updateM3UFileRequest = {} as UpdateM3UFileRequest;

    updateM3UFileRequest.id = m3uFileDto.id;
    updateM3UFileRequest.vodTags = vodTags;

    await M3UFilesUpdateM3UFileMutation(updateM3UFileRequest)
      .then(() => {})
      .catch(() => {
        console.log(`Error Updating M3U`);
      });
  };

  const buttonTags = useMemo((): string => {
    if (m3uFileDto.vodTags && m3uFileDto.vodTags.length > 0) {
      if (m3uFileDto.vodTags.length < 3) return m3uFileDto.vodTags.join(', ');
    }

    return m3uFileDto.vodTags.length + ' Tags';
  }, [m3uFileDto.vodTags]);

  return (
    <div className="w-full" ref={anchorReference}>
      <Button
        className="text-sm"
        icon="pi pi-chevron-down"
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
      />
      <OverlayPanel ref={op} onHide={() => setIsOpen(false)} style={{ width: '20rem' }}>
        <Chips
          autoFocus
          id="chips"
          value={m3uFileDto.vodTags ?? []}
          onChange={(e) => {
            updateM3U(e.value ?? []);
          }}
        />
      </OverlayPanel>
    </div>
  );
};

M3UFileTags.displayName = 'M3UFileDialog';

export default React.memo(M3UFileTags);
