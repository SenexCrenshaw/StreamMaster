import { GetEPGColors } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { GetEPGFilePreviewById } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { GetEPGNextEPGNumber } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetEPGColors = createAsyncThunk('cache/getGetEPGColors', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetEPGColors');
    const response = await GetEPGColors();
    console.log('Fetched GetEPGColors ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

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

export const fetchGetEPGNextEPGNumber = createAsyncThunk('cache/getGetEPGNextEPGNumber', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetEPGNextEPGNumber');
    const response = await GetEPGNextEPGNumber();
    console.log('Fetched GetEPGNextEPGNumber ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

