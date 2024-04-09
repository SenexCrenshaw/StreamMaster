import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import NumberInput from '@components/inputs/NumberInput';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { InputText } from 'primereact/inputtext';
import { ToggleButton } from 'primereact/togglebutton';
import React, { useCallback, useMemo } from 'react';
import M3UFileTags from './M3UFileTags';

export interface M3UFileDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly selectedFile: M3UFileDto;
  readonly noButtons?: boolean;
  readonly onM3UChanged?: (m3uFileDto: M3UFileDto) => void;
}

const M3UFileDialog = ({ onM3UChanged, selectedFile, noButtons }: M3UFileDialogProperties) => {
  const defaultValues = {
    Name: '',
    MaxStreamCount: 1,
    HoursToUpdate: 72,
    OverwriteChannelNumbers: true,
    StartingChannelNumber: 1
  } as M3UFileDto;

  const [m3uFileDto, setM3UFileDto] = React.useState<M3UFileDto>(defaultValues);
  const [originalM3UFileDto, setOriginalM3UFileDto] = React.useState<M3UFileDto | undefined>(undefined);

  const setName = useCallback(
    (value: string) => {
      if (m3uFileDto && m3uFileDto.Name !== value) {
        const m3uFileDtoCopy = { ...m3uFileDto };
        m3uFileDtoCopy.Name = value;
        setM3UFileDto(m3uFileDtoCopy);
        onM3UChanged && onM3UChanged(m3uFileDtoCopy);
      }
    },
    [m3uFileDto.Name]
  );

  const setMaxStreams = (value: number) => {
    if (m3uFileDto && m3uFileDto.MaxStreamCount !== value) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.MaxStreamCount = value;
      setM3UFileDto(m3uFileDtoCopy);
      onM3UChanged && onM3UChanged(m3uFileDtoCopy);
    }
  };

  const setAutoUpdate = (value: number) => {
    if (m3uFileDto && m3uFileDto.HoursToUpdate !== value) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.HoursToUpdate = value;
      setM3UFileDto(m3uFileDtoCopy);
      onM3UChanged && onM3UChanged(m3uFileDtoCopy);
    }
  };

  const setOverwriteChannelNumbers = (value: boolean) => {
    if (m3uFileDto && m3uFileDto.OverwriteChannelNumbers !== value) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.OverwriteChannelNumbers = value;
      setM3UFileDto(m3uFileDtoCopy);
      onM3UChanged && onM3UChanged(m3uFileDtoCopy);
    }
  };

  const setStartingChannelNumber = (value: number) => {
    if (m3uFileDto && m3uFileDto.StartingChannelNumber !== value) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.StartingChannelNumber = value;
      setM3UFileDto(m3uFileDtoCopy);
      onM3UChanged && onM3UChanged(m3uFileDtoCopy);
    }
  };

  const setVodTags = (value: string[]) => {
    if (m3uFileDto && m3uFileDto.VODTags !== value) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.VODTags = value;
      setM3UFileDto(m3uFileDtoCopy);
      onM3UChanged && onM3UChanged(m3uFileDtoCopy);
    }
  };

  const ReturnToParent = useCallback(() => {}, []);

  React.useEffect(() => {
    if (selectedFile === undefined) {
      return;
    }

    if (originalM3UFileDto === undefined) {
      setM3UFileDto(selectedFile);
      setOriginalM3UFileDto(selectedFile);
      return;
    }

    if (selectedFile !== m3uFileDto) {
      setM3UFileDto(selectedFile);
    }
  }, [selectedFile]);

  const isSaveEnabled = useMemo(() => {
    return m3uFileDto !== originalM3UFileDto;
  }, [m3uFileDto, originalM3UFileDto]);

  return (
    <>
      <div className="col-12">
        <div className="flex">
          <div className="col-6 p-0">
            <div id="name" className="text-xs text-500">
              NAME:
            </div>
            <InputText autoFocus className="p-float-label w-full" id="name" value={m3uFileDto.Name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className="col-6 p-0 pt-2">
            <div className="flex flex-wrap p-fluid align-items-center justify-content-between">
              <div className="col-6">
                <NumberInput
                  showButtons
                  label="MAX STREAMS"
                  onChange={(e) => {
                    setMaxStreams(e);
                  }}
                  showClear
                  value={m3uFileDto?.MaxStreamCount}
                />
              </div>
              <div className="col-6">
                <NumberInput
                  label="AUTO UPDATE"
                  onChange={(e) => {
                    setAutoUpdate(e);
                  }}
                  showClear
                  suffix=" Hours"
                  value={m3uFileDto?.HoursToUpdate}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="col-12">
        <div className="flex flex-wrap p-fluid align-items-center justify-content-between">
          <div className="sourceOrFileDialog-toggle pb-4">
            <div className="flex flex-column">
              <div id="name" className="text-xs text-500 pb-1">
                AUTO SET CHANNEL #S:
              </div>
              <ToggleButton checked={m3uFileDto?.OverwriteChannelNumbers} onChange={(e) => setOverwriteChannelNumbers(e.value)} />
            </div>
          </div>

          <div className="col-3">
            <NumberInput
              label="STARTING CHANNEL #"
              onChange={(e) => {
                setStartingChannelNumber(e);
              }}
              showClear
              value={m3uFileDto?.StartingChannelNumber}
            />
          </div>

          <div className="col-6">
            <M3UFileTags vodTags={m3uFileDto?.VODTags} onChange={(e) => setVodTags(e)} />
          </div>
        </div>
      </div>

      {noButtons !== true && (
        <div className="flex w-12 gap-2 justify-content-end align-content-center">
          <ResetButton
            disabled={!isSaveEnabled && originalM3UFileDto !== undefined}
            label="Reset"
            onClick={() => originalM3UFileDto !== undefined && setM3UFileDto(originalM3UFileDto)}
          />
          <SaveButton disabled={!isSaveEnabled} label="Update M3U" onClick={ReturnToParent} />
        </div>
      )}
    </>
  );
};

M3UFileDialog.displayName = 'M3UFileEditDialog';

export default React.memo(M3UFileDialog);
