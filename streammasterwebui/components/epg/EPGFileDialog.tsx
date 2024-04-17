import ColorEditor from '@components/ColorEditor';
import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import NumberInput from '@components/inputs/NumberInput';
import TextInput from '@components/inputs/TextInput';

import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { EPGFileDto, UpdateEPGFileRequest, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useMemo } from 'react';

export interface EPGFileDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly selectedFile: EPGFileDto;
  readonly noButtons?: boolean;
  readonly onEPGChanged?: (epgFileDto: EPGFileDto) => void;
  readonly onUpdated?: (request: UpdateM3UFileRequest) => void;
}

const EPGFileDialog = ({ onEPGChanged, onUpdated, selectedFile, noButtons }: EPGFileDialogProperties) => {
  const defaultValues = {
    Color: '',
    EPGNumber: 1,
    HoursToUpdate: 72,
    Name: '',
    TimeShift: 0
  } as EPGFileDto;

  const { code } = useScrollAndKeyEvents();

  const [epgFileDto, setEPGFileDto] = React.useState<EPGFileDto>(defaultValues);
  const [originalEPGFileDto, setOriginalEPGFileDto] = React.useState<EPGFileDto | undefined>(undefined);
  const [request, setRequest] = React.useState<UpdateEPGFileRequest>({} as UpdateEPGFileRequest);

  // useEffect(() => {
  //   if (epgFileDto?.Id !== selectedFile.Id) {
  //   }
  // }, []);

  const setColor = useCallback(
    (value: string) => {
      if (epgFileDto && epgFileDto.Color !== value) {
        const epgFileDtoCopy = { ...epgFileDto };
        epgFileDtoCopy.Color = value;
        setEPGFileDto(epgFileDtoCopy);

        const requestCopy = { ...request };
        requestCopy.Id = epgFileDtoCopy.Id;
        requestCopy.Color = value;
        setRequest(requestCopy);

        onEPGChanged && onEPGChanged(epgFileDtoCopy);
      }
    },
    [epgFileDto, onEPGChanged, request]
  );

  const setEPGNumber = (value: number) => {
    if (epgFileDto && epgFileDto.EPGNumber !== value) {
      const epgFileDtoCopy = { ...epgFileDto };
      epgFileDtoCopy.EPGNumber = value;
      setEPGFileDto(epgFileDtoCopy);

      const requestCopy = { ...request };
      requestCopy.Id = epgFileDtoCopy.Id;
      requestCopy.EPGNumber = value;
      setRequest(requestCopy);

      onEPGChanged && onEPGChanged(epgFileDtoCopy);
    }
  };

  const setHoursToUpdate = (value: number) => {
    if (epgFileDto && epgFileDto.HoursToUpdate !== value) {
      const epgFileDtoCopy = { ...epgFileDto };
      epgFileDtoCopy.HoursToUpdate = value;
      setEPGFileDto(epgFileDtoCopy);

      const requestCopy = { ...request };
      requestCopy.Id = epgFileDtoCopy.Id;
      requestCopy.HoursToUpdate = value;
      setRequest(requestCopy);

      onEPGChanged && onEPGChanged(epgFileDtoCopy);
    }
  };

  const setName = useCallback(
    (value: string) => {
      if (epgFileDto && epgFileDto.Name !== value) {
        const epgFileDtoCopy = { ...epgFileDto };
        epgFileDtoCopy.Name = value;
        setEPGFileDto(epgFileDtoCopy);

        const requestCopy = { ...request };
        requestCopy.Id = epgFileDtoCopy.Id;
        requestCopy.Name = value;
        setRequest(requestCopy);

        onEPGChanged && onEPGChanged(epgFileDtoCopy);
      }
    },
    [epgFileDto, onEPGChanged, request]
  );

  const setTimeShift = (value: number) => {
    if (epgFileDto && epgFileDto.TimeShift !== value) {
      const epgFileDtoCopy = { ...epgFileDto };
      epgFileDtoCopy.TimeShift = value;
      setEPGFileDto(epgFileDtoCopy);

      const requestCopy = { ...request };
      requestCopy.Id = epgFileDtoCopy.Id;
      requestCopy.TimeShift = value;
      setRequest(requestCopy);

      onEPGChanged && onEPGChanged(epgFileDtoCopy);
    }
  };

  const isSaveEnabled = useMemo(() => {
    if (epgFileDto.Color !== originalEPGFileDto?.Color) {
      return true;
    }

    if (epgFileDto.EPGNumber !== originalEPGFileDto.EPGNumber) {
      return true;
    }

    if (epgFileDto.HoursToUpdate !== originalEPGFileDto?.HoursToUpdate) {
      return true;
    }

    if (epgFileDto.Name !== originalEPGFileDto?.Name) {
      return true;
    }

    if (epgFileDto.TimeShift !== originalEPGFileDto?.TimeShift) {
      return true;
    }

    return false;
  }, [epgFileDto, originalEPGFileDto]);

  if (code === 'Enter' || code === 'NumpadEnter') {
    if (isSaveEnabled) {
      onUpdated && onUpdated(request);
    }
  }

  useEffect(() => {
    if (selectedFile === undefined) {
      return;
    }

    if (originalEPGFileDto === undefined) {
      setEPGFileDto(selectedFile);
      setOriginalEPGFileDto(selectedFile);
      return;
    }

    if (
      selectedFile.Id === epgFileDto.Id &&
      selectedFile.Name !== epgFileDto.Name &&
      (originalEPGFileDto.Name === '' || epgFileDto.Name === originalEPGFileDto.Name)
    ) {
      setName(selectedFile.Name);
      return;
    }

    return;
  }, [epgFileDto.Id, epgFileDto.Name, originalEPGFileDto, selectedFile, setName]);

  return (
    <>
      <div className="col-12 ">
        <div className="flex">
          <div className="col-6 pt-2">
            <div className="sm-inputnumber-input-no-right-border col-12">
              <TextInput autoFocus value={epgFileDto.Name} label="Name" onChange={(e) => setName(e)} />
            </div>
          </div>
          <div className="col-6 p-0 pt-2">
            <div className="flex p-fluid align-items-center justify-content-between">
              <div className="col-6">
                <NumberInput
                  label="AUTO UPDATE"
                  onChange={(e) => {
                    setHoursToUpdate(e);
                  }}
                  showClear
                  suffix=" Hours"
                  value={epgFileDto.HoursToUpdate}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="col-12">
        <div className="flex flex-wrap p-fluid align-items-center justify-content-between">
          <div className="col-3">
            <ColorEditor
              onChange={async (e) => {
                console.log(e);
                setColor(e);
              }}
              color={epgFileDto.Color}
            />
          </div>
          <div className="sm-inputnumber-input-no-right-border col-4">
            <NumberInput
              showButtons
              label="EPGNumber"
              onChange={(e) => {
                setEPGNumber(e);
              }}
              showClear
              value={epgFileDto.EPGNumber}
            />
          </div>
          <div className="col-3">
            <NumberInput
              label="Time Shift"
              onChange={(e) => {
                setTimeShift(e);
              }}
              showClear
              value={epgFileDto.TimeShift}
            />
          </div>
        </div>
      </div>

      {noButtons !== true && (
        <div className="flex w-12 gap-2 justify-content-end align-content-center pr-3">
          <ResetButton
            disabled={!isSaveEnabled && originalEPGFileDto !== undefined}
            onClick={() => {
              if (originalEPGFileDto !== undefined) {
                setEPGFileDto(originalEPGFileDto);
                setColor(originalEPGFileDto.Color);
              }
            }}
          />
          <SaveButton disabled={!isSaveEnabled} label="Update EPG" onClick={() => onUpdated && onUpdated(request)} />
        </div>
      )}
    </>
  );
};

EPGFileDialog.displayName = 'EPGFileDialog';

export default React.memo(EPGFileDialog);
