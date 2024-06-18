import ColorEditor from '@components/inputs/ColorEditor';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import { Logger } from '@lib/common/logger';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';

import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo } from 'react';

export interface EPGFileDialogProperties {
  readonly onEPGChanged?: (epgFileDto: EPGFileDto) => void;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
  readonly selectedFile: EPGFileDto;
}

export interface EPGFileDialogRef {
  reset: () => void;
  save: () => void;
}

const EPGFileDialog = forwardRef<EPGFileDialogRef, EPGFileDialogProperties>(({ onEPGChanged, onSaveEnabled, selectedFile }, ref) => {
  const defaultValues = useMemo(
    () =>
      ({
        Color: '',
        EPGNumber: 1,
        HoursToUpdate: 72,
        Name: '',
        TimeShift: 0
      } as EPGFileDto),
    []
  );

  const { code } = useScrollAndKeyEvents();
  const [epgFileDto, setEPGFileDto] = React.useState<EPGFileDto>(defaultValues);
  const [originalEPGFileDto, setOriginalEPGFileDto] = React.useState<EPGFileDto | undefined>(undefined);
  const [request, setRequest] = React.useState<UpdateEPGFileRequest>({} as UpdateEPGFileRequest);
  const { setStringValue } = useStringValue('epgName');

  const onUpdated = useCallback(async () => {
    if (request.Id === undefined) {
      return;
    }

    try {
      await UpdateEPGFile(request);
    } catch (error) {
      console.error(error);
    }
  }, [request]);

  useImperativeHandle(
    ref,
    () => ({
      reset: () => {
        if (originalEPGFileDto) {
          setEPGFileDto({ ...originalEPGFileDto });
        } else {
          setEPGFileDto(defaultValues);
        }
      },
      save: () => {
        onUpdated();
      }
    }),
    [defaultValues, onUpdated, originalEPGFileDto]
  );

  const isSaveEnabled = useMemo(() => {
    if (epgFileDto === undefined) {
      return false;
    }

    let isChanged = Object.keys(defaultValues).some((key) => epgFileDto[key as keyof EPGFileDto] !== originalEPGFileDto?.[key as keyof EPGFileDto]);

    onSaveEnabled?.(isChanged);
    return isChanged;
  }, [epgFileDto, defaultValues, onSaveEnabled, originalEPGFileDto]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<EPGFileDto>) => {
      if (epgFileDto === undefined) {
        return;
      }
      const updatedEPGFileDto = { ...epgFileDto, ...updatedFields };
      const updatedRequest = { ...request, Id: updatedEPGFileDto.Id, ...updatedFields };

      setEPGFileDto(updatedEPGFileDto);
      setRequest(updatedRequest);

      onEPGChanged && onEPGChanged(updatedEPGFileDto);
    },
    [epgFileDto, request, onEPGChanged]
  );

  useEffect(() => {
    if (selectedFile && (!originalEPGFileDto || selectedFile.Id !== originalEPGFileDto.Id)) {
      setEPGFileDto(selectedFile);
      setOriginalEPGFileDto(selectedFile);
    }
  }, [selectedFile, originalEPGFileDto]);

  useEffect(() => {
    if (code === 'Enter' || code === 'NumpadEnter') {
      if (isSaveEnabled) {
        onUpdated();
      }
    }
  }, [code, isSaveEnabled, onUpdated]);

  if (epgFileDto !== undefined) {
    Logger.debug('EPGFileDialog', { isSaveEnabled, epgFileDto: epgFileDto.Url, originalEPGFileDto: originalEPGFileDto?.Url });
  }

  if (selectedFile === undefined) {
    return null;
  }

  // const setColor = useCallback(
  //   (value: string) => {
  //     if (epgFileDto && epgFileDto.Color !== value) {
  //       const epgFileDtoCopy = { ...epgFileDto };
  //       epgFileDtoCopy.Color = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.Color = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  // const setEPGNumber = useCallback(
  //   (value: number) => {
  //     if (epgFileDto && epgFileDto.EPGNumber !== value) {
  //       const epgFileDtoCopy = { ...epgFileDto };
  //       epgFileDtoCopy.EPGNumber = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.EPGNumber = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  // const setHoursToUpdate = useCallback(
  //   (value: number) => {
  //     if (epgFileDto && epgFileDto.HoursToUpdate !== value) {
  //       const epgFileDtoCopy = { ...epgFileDto };
  //       epgFileDtoCopy.HoursToUpdate = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.HoursToUpdate = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  // const setName = useCallback(
  //   (value: string) => {
  //     if (epgFileDto && epgFileDto.Name !== value) {
  //       const epgFileDtoCopy = { ...epgFileDto };
  //       epgFileDtoCopy.Name = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.Name = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  // const setUrl = useCallback(
  //   (value: string) => {
  //     if (epgFileDto && epgFileDto.Url !== value) {
  //       const epgFileDtoCopy = { ...epgFileDto };
  //       epgFileDtoCopy.Url = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.Url = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  // const setTimeShift = useCallback(
  //   (value: number) => {
  //     if (epgFileDto && epgFileDto.TimeShift !== value) {
  //       const epgFileDtoCopy = { ...epgFileDto };
  //       epgFileDtoCopy.TimeShift = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.TimeShift = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  // const isSaveEnabled = useMemo(() => {
  //   if (epgFileDto.Color !== originalEPGFileDto?.Color) {
  //     return true;
  //   }

  //   if (epgFileDto.EPGNumber !== originalEPGFileDto.EPGNumber) {
  //     return true;
  //   }

  //   if (epgFileDto.HoursToUpdate !== originalEPGFileDto?.HoursToUpdate) {
  //     return true;
  //   }

  //   if (epgFileDto.Name !== originalEPGFileDto?.Name) {
  //     return true;
  //   }

  //   if (epgFileDto.Url !== originalEPGFileDto?.Url) {
  //     return true;
  //   }

  //   if (epgFileDto.TimeShift !== originalEPGFileDto?.TimeShift) {
  //     return true;
  //   }

  //   return false;
  // }, [epgFileDto, originalEPGFileDto]);

  // if (code === 'Enter' || code === 'NumpadEnter') {
  //   if (isSaveEnabled) {
  //     onUpdated();
  //   }
  // }

  // useEffect(() => {
  //   if (selectedFile === undefined) {
  //     return;
  //   }

  //   if (originalEPGFileDto === undefined) {
  //     setEPGFileDto(selectedFile);
  //     setOriginalEPGFileDto(selectedFile);
  //     return;
  //   }

  //   if (
  //     selectedFile.Id === epgFileDto.Id &&
  //     selectedFile.Name !== epgFileDto.Name &&
  //     (originalEPGFileDto.Name === '' || epgFileDto.Name === originalEPGFileDto.Name)
  //   ) {
  //     setName(selectedFile.Name);
  //     return;
  //   }

  //   return;
  // }, [epgFileDto.Id, epgFileDto.Name, originalEPGFileDto, selectedFile, setName]);

  return (
    <>
      <div className="w-12">
        <div className="flex gap-2">
          <div className="w-8">
            <StringEditor
              disableDebounce
              darkBackGround
              autoFocus
              label="NAME"
              value={epgFileDto?.Name}
              onChange={(e) => {
                updateStateAndRequest({ Name: e });
                setStringValue(e);
                // e !== undefined && setName(e);
              }}
              onSave={(e) => {}}
            />
          </div>
          <div className="w-2">
            <div className="sm-sourceorfiledialog-toggle">
              <div className="flex flex-column">
                <ColorEditor
                  color={epgFileDto.Color}
                  label="Color"
                  // onChange={async (e) => {

                  //   setColor(e);
                  // }}
                  onChange={(e) => updateStateAndRequest({ Color: e })}
                />
              </div>
            </div>
          </div>
          <div className="w-2">
            <NumberEditor
              disableDebounce
              darkBackGround
              showButtons
              label="EPG #"
              // onSave={(e) => {
              //   e !== undefined && setEPGNumber(e);
              // }}
              onChange={(e) => updateStateAndRequest({ EPGNumber: e })}
              value={epgFileDto.EPGNumber}
            />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg" />

      {/* <div className="w-12">
        <StringEditor
          showClear
          disableDebounce
          darkBackGround
          label="URL"
          value={epgFileDto?.Url}
          // onChange={(e) => {
          //   e !== undefined && setUrl(e);
          // }}
          // onSave={(e) => {}}
          onChange={(e) => updateStateAndRequest({ Url: e })}
        />
      </div> */}
      {/* <div className="layout-padding-bottom-lg" /> */}

      <div className="w-12">
        <div className="flex gap-2">
          <div className="w-4">
            <NumberEditor
              disableDebounce
              darkBackGround
              label="TIME SHIFT"
              // onSave={(e) => {
              //   e !== undefined && setTimeShift(e);
              // }}
              onChange={(e) => updateStateAndRequest({ TimeShift: e })}
              value={epgFileDto.TimeShift}
            />
          </div>
          <div className="w-4">
            <NumberEditor
              disableDebounce
              darkBackGround
              label="AUTO UPDATE"
              // onSave={(e) => {
              //   e !== undefined && setHoursToUpdate(e);
              // }}
              onChange={(e) => updateStateAndRequest({ HoursToUpdate: e })}
              suffix=" Hours"
              value={epgFileDto.HoursToUpdate}
            />
          </div>
        </div>
      </div>
    </>
  );
});

EPGFileDialog.displayName = 'EPGFileDialog';

export default React.memo(EPGFileDialog);
