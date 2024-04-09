import { FieldData, PagedResponse } from '@lib/smAPI/smapiTypes';

export const updatePagedResponseFieldInData = (pagedResponse: PagedResponse<any> | undefined, fieldData: FieldData): PagedResponse<any> | undefined => {
  if (!pagedResponse) return undefined;

  if (fieldData.Field === 'SMStreams') {
    const updatedPagedResponse = {
      ...pagedResponse,
      data: pagedResponse.Data.map((dto) => {
        const id = dto.Id.toString();
        if (id === fieldData.Id && fieldData.Field && dto[fieldData.Field] !== fieldData.Value) {
          const updatedSMSTreams = Array.isArray(fieldData.Value) ? [...fieldData.Value] : fieldData.Value;
          return {
            ...dto,
            SMSTreams: updatedSMSTreams // Ensuring SMSTreams is a new instance
          };
        }
        return dto;
      })
    };
    return updatedPagedResponse;
  }

  const updatedPagedResponse = {
    ...pagedResponse,
    Data: pagedResponse.Data.map((dto) => {
      const id = dto.Id.toString();
      if (id === fieldData.Id && fieldData.Field && dto[fieldData.Field] !== fieldData.Value) {
        var test = {
          ...dto,
          [fieldData.Field]: fieldData.Value
        };
        return test;
      }
      return dto;
    })
  };

  return updatedPagedResponse;
};
