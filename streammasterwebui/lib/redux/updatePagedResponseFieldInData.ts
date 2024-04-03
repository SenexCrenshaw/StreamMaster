import { FieldData } from '@lib/apiDefs';
import { PagedResponse } from '@lib/smAPI/smapiTypes';

export const updatePagedResponseFieldInData = (pagedResponse: PagedResponse<any> | undefined, fieldData: FieldData): PagedResponse<any> | undefined => {
  if (!pagedResponse) return undefined;

  const updatedPagedResponse = {
    ...pagedResponse,
    data: pagedResponse.data.map((dto) => {
      const id = dto.id.toString();
      if (id === fieldData.id && dto[fieldData.field] !== fieldData.value) {
        var test = {
          ...dto,
          [fieldData.field]: fieldData.value
        };
        return test;
      }
      return dto;
    })
  };

  return updatedPagedResponse;
};
