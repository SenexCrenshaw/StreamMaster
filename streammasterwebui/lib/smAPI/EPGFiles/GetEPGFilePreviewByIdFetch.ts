import { GetEPGFilePreviewById } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGFilePreviewById = createAsyncThunk('cache/getGetEPGFilePreviewById', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetEPGFilePreviewById');
    const response = await GetEPGFilePreviewById();
    console.log('Fetched GetEPGFilePreviewById ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});


