import { FieldData, PagedResponse } from '@lib/apiDefs';

export const updatePagedResponseFieldInData = (pagedResponse: PagedResponse<any> | undefined, fieldData: FieldData): PagedResponse<any> | undefined => {
  if (!pagedResponse) return undefined;

  const updatedPagedResponse = {
    ...pagedResponse,
    data: pagedResponse.data.map((dto) => {
      if (dto.id === fieldData.id) {
        return {
          ...dto,
          [fieldData.field]: fieldData.value // Dynamically update the specified field
        };
      }
      return dto;
    })
  };

  return updatedPagedResponse;
};
