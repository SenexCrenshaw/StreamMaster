import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { memo, useEffect, useMemo, useRef, useState } from 'react';
import M3UFileDialog, { M3UFileDialogRef } from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const defaultValues = useMemo(
    () =>
      ({
        HoursToUpdate: 72,
        MaxStreamCount: 1,
        Name: '',
        OverwriteChannelNumbers: true,
        StartingChannelNumber: 1
      } as M3UFileDto),
    []
  );

  const m3uDialogRef = useRef<M3UFileDialogRef>(null);
  const smDialogRef = useRef<SMDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const [m3uFileDto, setM3UFileDto] = useState<M3UFileDto>(defaultValues);
  const [originalM3UFileDto, setOriginalM3UFileDto] = useState<M3UFileDto | undefined>(undefined);

  useEffect(() => {
    if (selectedFile === undefined) {
      return;
    }

    setM3UFileDto(selectedFile);
    setOriginalM3UFileDto(selectedFile);

    return;
  }, [selectedFile]);

  return (
    <SMDialog
      ref={smDialogRef}
      widthSize={5}
      position="top-right"
      title="EDIT M3U"
      icon="pi-pencil"
      iconFilled={false}
      buttonClassName="icon-yellow"
      tooltip="Edit M3U"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            disabled={!saveEnabled && originalM3UFileDto !== undefined}
            onClick={() => {
              m3uDialogRef.current?.reset();
            }}
          />
          <OKButton
            disabled={!saveEnabled}
            onClick={(request) => {
              m3uDialogRef.current?.save();
              smDialogRef.current?.hide();
            }}
          />
        </div>
      }
    >
      <M3UFileDialog ref={m3uDialogRef} selectedFile={m3uFileDto} onSaveEnabled={setSaveEnabled} />
    </SMDialog>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
