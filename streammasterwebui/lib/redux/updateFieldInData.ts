import { FieldData } from '@lib/apiDefs';

export const updateFieldInData = (response: any | undefined, fieldData: FieldData): any | undefined => {
  if (!response) return undefined;

  const updatedResponse = response.map((dto: any) => {
    const id = dto.id.toString();
    if (id === fieldData.id) {
      var test = {
        ...dto,
        [fieldData.field]: fieldData.value
      };
      return test;
    }
    return dto;
  });

  return updatedResponse;
};
